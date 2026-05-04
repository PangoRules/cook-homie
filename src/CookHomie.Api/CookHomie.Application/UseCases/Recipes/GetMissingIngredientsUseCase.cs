using CookHomie.Domain.Interfaces;

namespace CookHomie.Application.UseCases.Recipes;

public class GetMissingIngredientsUseCase
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public GetMissingIngredientsUseCase(
        IRecipeRepository recipeRepository,
        IInventoryRepository inventoryRepository)
    {
        _recipeRepository = recipeRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IReadOnlyList<string>?> ExecuteAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe is null)
        {
            return null;
        }

        var inventory = await _inventoryRepository.GetAllAsync(cancellationToken);
        var inventoryNames = inventory
            .Select(i => i.Name.Trim().ToUpperInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = recipe.Ingredients
            .Where(i => !i.IsOptional)
            .Select(i => i.IngredientName.Trim().ToUpperInvariant())
            .Where(name => !inventoryNames.Contains(name))
            .ToList();

        return missing;
    }
}