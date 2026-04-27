using System.Net;
using System.Net.Http.Json;
using CookHomie.Domain.Entities;
using CookHomie.Domain.Enums;
using CookHomie.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CookHomie.WebApi.Tests;

public class InventoryControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly InMemoryDatabaseRoot _databaseRoot = new();

    public InventoryControllerTests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase("webapi-test-db", _databaseRoot)
                );
            });
        });

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        db.InventoryItems.RemoveRange(db.InventoryItems);
        db.InventoryItems.AddRange(
            new InventoryItem
            {
                Id = Guid.NewGuid(),
                Name = "Seed Milk",
                Category = "Dairy",
                Location = Location.Fridge,
                Quantity = 1,
                Unit = "liter",
                IsOpened = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new InventoryItem
            {
                Id = Guid.NewGuid(),
                Name = "Seed Pasta",
                Category = "Grains",
                Location = Location.Pantry,
                Quantity = 2,
                Unit = "packs",
                IsOpened = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            }
        );
        db.SaveChanges();
    }

    [Fact]
    public async Task GetInventory_ReturnsSeededItems()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/inventory");
        var items = await response.Content.ReadFromJsonAsync<List<InventoryItemResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.True(items.Count >= 2);
    }

    [Fact]
    public async Task PostInventory_PersistsAndReturnsCreatedItem()
    {
        var client = _factory.CreateClient();

        var payload = new CreateInventoryItemRequest
        {
            Name = "Eggs",
            Category = "Dairy",
            Location = "Fridge",
            Quantity = 12,
            Unit = "units",
            IsOpened = false,
        };

        var postResponse = await client.PostAsJsonAsync("/api/inventory", payload);
        var created = await postResponse.Content.ReadFromJsonAsync<InventoryItemResponse>();

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);
        Assert.Contains("api/inventory", postResponse.Headers.Location!.ToString());
        Assert.NotNull(created);
        Assert.Equal("Eggs", created!.Name);
        Assert.Equal("Dairy", created.Category);
        Assert.Equal("Fridge", created.Location);
        Assert.Equal(12, created.Quantity);
        Assert.Equal("units", created.Unit);
        Assert.False(created.IsOpened);

        var listResponse = await client.GetAsync("/api/inventory");
        var items = await listResponse.Content.ReadFromJsonAsync<List<InventoryItemResponse>>();
        Assert.NotNull(items);
        Assert.Contains(items!, i => i.Id == created.Id);
    }

    [Fact]
    public async Task PostInventory_WithInvalidPayload_ReturnsProblemDetails()
    {
        var client = _factory.CreateClient();
        var payload = new CreateInventoryItemRequest
        {
            Name = "",
            Category = "",
            Location = "",
            Quantity = 0,
            Unit = "",
            IsOpened = false,
        };

        var response = await client.PostAsJsonAsync("/api/inventory", payload);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            "application/problem+json",
            response.Content.Headers.ContentType?.ToString()
        );
        Assert.Contains("Invalid inventory payload", body);
    }

    [Fact]
    public async Task Health_ReturnsApiAndDatabaseChecks()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"status\":\"ok\"", body);
        Assert.Contains("\"api\":\"ok\"", body);
        Assert.Contains("\"database\":\"ok\"", body);
    }

    [Fact]
    public async Task OpenApi_DoesNotListSwaggerUiRoutes()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("\"/swagger\"", body);
        Assert.DoesNotContain("\"/swagger/index.html\"", body);
    }

    public sealed class CreateInventoryItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsOpened { get; set; }
    }

    public sealed class InventoryItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateOnly? ExpiresAt { get; set; }
        public bool IsOpened { get; set; }
        public string? Notes { get; set; }
    }
}
