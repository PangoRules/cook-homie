using CookHomie.Application.DTOs;
using CookHomie.Application.Mappings;
using CookHomie.Domain.Interfaces;

namespace CookHomie.Application.UseCases.Recipes;

public class GetRecipesUseCase(
    IRecipeRepository recipeRepository,
    IInventoryRepository inventoryRepository
)
{
    public async Task<IReadOnlyList<RecipeDto>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        var recipes = await recipeRepository.GetAllAsync(cancellationToken);
        return [.. recipes.Select(r => r.ToDto())];
    }

    public async Task<RecipeDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var recipe = await recipeRepository.GetByIdAsync(id, cancellationToken);
        return recipe?.ToDto();
    }

    public async Task<RecipeStockCheckDto?> GetStockCheckAsync(
        Guid recipeId,
        CancellationToken cancellationToken = default
    )
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe is null)
        {
            return null;
        }

        var requiredIngredients = recipe
            .Ingredients.Where(i => !i.IsOptional)
            .ToList();
        var recipeIngredientNames = requiredIngredients.Select(i => i.IngredientName.Trim()).ToList();

        var matchingInventory = await inventoryRepository.GetByNamesAsync(recipeIngredientNames, cancellationToken);
        var inventoryByNameAndUnit = matchingInventory
            .GroupBy(i => (Name: Normalize(i.Name), Unit: Normalize(i.Unit)))
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        var items = requiredIngredients
            .Select(i =>
            {
                var availableQuantity = inventoryByNameAndUnit.GetValueOrDefault(
                    (Normalize(i.IngredientName), Normalize(i.Unit))
                );
                var status = availableQuantity <= 0
                    ? "Missing"
                    : availableQuantity < i.Quantity
                        ? "Insufficient"
                        : "Good";

                return new RecipeStockItemDto
                {
                    IngredientName = i.IngredientName,
                    RequiredQuantity = i.Quantity,
                    AvailableQuantity = availableQuantity,
                    Unit = i.Unit,
                    Status = status,
                };
            })
            .ToList();

        return new RecipeStockCheckDto
        {
            RecipeId = recipe.Id,
            CanCook = items.All(i => i.Status == "Good"),
            MissingCount = items.Count(i => i.Status == "Missing"),
            InsufficientCount = items.Count(i => i.Status == "Insufficient"),
            GoodCount = items.Count(i => i.Status == "Good"),
            Items = items,
        };
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
