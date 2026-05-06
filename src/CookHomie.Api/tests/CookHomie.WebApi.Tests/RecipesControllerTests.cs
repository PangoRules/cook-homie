using System.Net;
using System.Net.Http.Json;
using CookHomie.Domain.Entities;
using CookHomie.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CookHomie.WebApi.Tests;

public class RecipesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly InMemoryDatabaseRoot _databaseRoot = new();
    private readonly string _databaseName = $"recipes-api-test-db-{Guid.NewGuid()}";

    public RecipesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                );

                if (dbDescriptor is not null)
                {
                    services.Remove(dbDescriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName, _databaseRoot)
                );
            });
        });

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        db.RecipeIngredients.RemoveRange(db.RecipeIngredients);
        db.Recipes.RemoveRange(db.Recipes);
        db.InventoryItems.RemoveRange(db.InventoryItems);

        var recipeId = Guid.NewGuid();
        db.Recipes.Add(new Recipe
        {
            Id = recipeId,
            Name = "Seed Pancakes",
            Instructions = "Mix flour, eggs, milk. Cook on griddle.",
            PrepMinutes = 5,
            CookMinutes = 10,
            Tags = ["breakfast", "quick"],
            Source = null,
            CreatedAt = DateTime.UtcNow,
            Ingredients = new List<RecipeIngredient>
            {
                new RecipeIngredient
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipeId,
                    IngredientName = "flour",
                    Quantity = 200,
                    Unit = "g",
                    IsOptional = false
                },
                new RecipeIngredient
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipeId,
                    IngredientName = "eggs",
                    Quantity = 2,
                    Unit = "pcs",
                    IsOptional = false
                }
            }
        });

        db.InventoryItems.Add(new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = "Milk",
            Category = "Dairy",
            Location = CookHomie.Domain.Enums.Location.Fridge,
            Quantity = 1,
            Unit = "liter",
            IsOpened = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        db.SaveChanges();
    }

    [Fact]
    public async Task GetRecipes_ReturnsPersistedRecipesWithIngredients()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/recipes");
        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(recipes);
        Assert.True(recipes.Count >= 1);
        var recipe = recipes.First(r => r.Name == "Seed Pancakes");
        Assert.True(recipe.Ingredients.Count >= 2);
    }

    [Fact]
    public async Task GetRecipe_WithKnownId_ReturnsRecipe()
    {
        var client = _factory.CreateClient();

        var getAllResponse = await client.GetAsync("/api/recipes");
        var recipes = await getAllResponse.Content.ReadFromJsonAsync<List<RecipeResponse>>();
        var seedRecipe = recipes!.First(r => r.Name == "Seed Pancakes");

        var response = await client.GetAsync($"/api/recipes/{seedRecipe.Id}");
        var recipe = await response.Content.ReadFromJsonAsync<RecipeResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(recipe);
        Assert.Equal("Seed Pancakes", recipe.Name);
    }

    [Fact]
    public async Task GetRecipe_WithMissingId_ReturnsNotFoundProblemDetails()
    {
        var client = _factory.CreateClient();
        var unknownId = Guid.NewGuid();

        var response = await client.GetAsync($"/api/recipes/{unknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(
            "application/problem+json",
            response.Content.Headers.ContentType?.ToString()
        );
    }

    [Fact]
    public async Task PostRecipe_PersistsRecipeAndIngredients()
    {
        var client = _factory.CreateClient();

        var payload = new UpsertRecipeRequest
        {
            Name = "Avocado Toast",
            Instructions = "Toast bread. Mash avocado with salt and lemon. Spread.",
            PrepMinutes = 3,
            CookMinutes = 2,
            Tags = ["breakfast", "quick"],
            Ingredients = new List<UpsertRecipeIngredientRequest>
            {
                new UpsertRecipeIngredientRequest
                {
                    IngredientName = "bread",
                    Quantity = 2,
                    Unit = "slices",
                    IsOptional = false
                },
                new UpsertRecipeIngredientRequest
                {
                    IngredientName = "avocado",
                    Quantity = 1,
                    Unit = "pcs",
                    IsOptional = false
                }
            }
        };

        var postResponse = await client.PostAsJsonAsync("/api/recipes", payload);
        var created = await postResponse.Content.ReadFromJsonAsync<RecipeResponse>();

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);
        Assert.Contains("api/recipes", postResponse.Headers.Location!.ToString());
        Assert.NotNull(created);
        Assert.Equal("Avocado Toast", created.Name);
        Assert.True(created.Ingredients.Count >= 2);
    }

    [Fact]
    public async Task PostRecipe_WithBlankName_ReturnsBadRequestProblemDetails()
    {
        var client = _factory.CreateClient();

        var payload = new UpsertRecipeRequest
        {
            Name = "   ",
            Instructions = "Some instructions",
            PrepMinutes = 0,
            CookMinutes = 0,
            Tags = [],
            Ingredients = new List<UpsertRecipeIngredientRequest>
            {
                new UpsertRecipeIngredientRequest
                {
                    IngredientName = "flour",
                    Quantity = 1,
                    Unit = "g",
                    IsOptional = false
                }
            }
        };

        var response = await client.PostAsJsonAsync("/api/recipes", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            "application/problem+json",
            response.Content.Headers.ContentType?.ToString()
        );
    }

    [Fact]
    public async Task PatchRecipe_ReplacesIngredientsWhenSupplied()
    {
        var client = _factory.CreateClient();

        var getAllResponse = await client.GetAsync("/api/recipes");
        var recipes = await getAllResponse.Content.ReadFromJsonAsync<List<RecipeResponse>>();
        var seedRecipe = recipes!.First(r => r.Name == "Seed Pancakes");

        var patchPayload = new UpsertRecipeRequest
        {
            Name = "Seed Pancakes Updated",
            Instructions = seedRecipe.Instructions,
            PrepMinutes = 10,
            CookMinutes = 15,
            Tags = ["breakfast", "updated"],
            Ingredients = new List<UpsertRecipeIngredientRequest>
            {
                new UpsertRecipeIngredientRequest
                {
                    IngredientName = "flour",
                    Quantity = 300,
                    Unit = "g",
                    IsOptional = false
                },
                new UpsertRecipeIngredientRequest
                {
                    IngredientName = "milk",
                    Quantity = 500,
                    Unit = "ml",
                    IsOptional = false
                }
            }
        };

var patchResponse = await client.PatchAsJsonAsync($"/api/recipes/{seedRecipe.Id}", patchPayload);
        var updated = await patchResponse.Content.ReadFromJsonAsync<RecipeResponse>();

        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);
        Assert.NotNull(updated);
        Assert.Equal("Seed Pancakes Updated", updated.Name);
        Assert.Equal(10, updated.PrepMinutes);
        Assert.Equal(15, updated.CookMinutes);
        Assert.True(updated.Ingredients.Count >= 2);
    }

    [Fact]
    public async Task GetRecipeStockCheck_ReturnsMissingAndGoodStatuses()
    {
        var client = _factory.CreateClient();

        var getAllResponse = await client.GetAsync("/api/recipes");
        var recipes = await getAllResponse.Content.ReadFromJsonAsync<List<RecipeResponse>>();
        var seedRecipe = recipes!.First(r => r.Name == "Seed Pancakes");

        var response = await client.GetAsync($"/api/recipes/{seedRecipe.Id}/stock-check");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var stockCheck = await response.Content.ReadFromJsonAsync<RecipeStockCheckResponse>();
        Assert.NotNull(stockCheck);
        Assert.False(stockCheck.CanCook);
        Assert.Equal(2, stockCheck.MissingCount);
        Assert.Equal(0, stockCheck.InsufficientCount);
        Assert.Equal(0, stockCheck.GoodCount);
        Assert.Contains(stockCheck.Items, i => i.IngredientName == "flour" && i.Status == "Missing");
        Assert.Contains(stockCheck.Items, i => i.IngredientName == "eggs" && i.Status == "Missing");
    }

    [Fact]
    public async Task GetRecipeStockCheck_ReturnsInsufficientWhenInventoryQuantityIsTooLow()
    {
        var client = _factory.CreateClient();

        var payload = new UpsertRecipeRequest
        {
            Name = "Milkshake",
            Instructions = "Blend milk.",
            PrepMinutes = 1,
            CookMinutes = 0,
            Tags = ["drink"],
            Ingredients =
            [
                new UpsertRecipeIngredientRequest
                {
                    IngredientName = "Milk",
                    Quantity = 2,
                    Unit = "liter",
                    IsOptional = false,
                },
            ],
        };

        var postResponse = await client.PostAsJsonAsync("/api/recipes", payload);
        var created = await postResponse.Content.ReadFromJsonAsync<RecipeResponse>();

        var response = await client.GetAsync($"/api/recipes/{created!.Id}/stock-check");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var stockCheck = await response.Content.ReadFromJsonAsync<RecipeStockCheckResponse>();
        Assert.NotNull(stockCheck);
        Assert.False(stockCheck.CanCook);
        Assert.Equal(0, stockCheck.MissingCount);
        Assert.Equal(1, stockCheck.InsufficientCount);
        Assert.Equal(0, stockCheck.GoodCount);
        Assert.Contains(
            stockCheck.Items,
            i =>
                i.IngredientName == "Milk"
                && i.RequiredQuantity == 2
                && i.AvailableQuantity == 1
                && i.Unit == "liter"
                && i.Status == "Insufficient"
        );
    }

    public sealed class UpsertRecipeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public int PrepMinutes { get; set; }
        public int CookMinutes { get; set; }
        public string[] Tags { get; set; } = [];
        public string? Source { get; set; }
        public List<UpsertRecipeIngredientRequest> Ingredients { get; set; } = [];
    }

    public sealed class UpsertRecipeIngredientRequest
    {
        public Guid? Id { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsOptional { get; set; }
    }

    public sealed class RecipeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public int PrepMinutes { get; set; }
        public int CookMinutes { get; set; }
        public string[] Tags { get; set; } = [];
        public string? Source { get; set; }
        public List<IngredientResponse> Ingredients { get; set; } = [];
    }

    public sealed class IngredientResponse
    {
        public Guid Id { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsOptional { get; set; }
    }

    public sealed class RecipeStockCheckResponse
    {
        public Guid RecipeId { get; set; }
        public bool CanCook { get; set; }
        public int MissingCount { get; set; }
        public int InsufficientCount { get; set; }
        public int GoodCount { get; set; }
        public List<RecipeStockItemResponse> Items { get; set; } = [];
    }

    public sealed class RecipeStockItemResponse
    {
        public string IngredientName { get; set; } = string.Empty;
        public decimal RequiredQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
