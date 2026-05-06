using CookHomie.Application.DTOs;
using CookHomie.Application.Mappings;
using CookHomie.Domain.Interfaces;

namespace CookHomie.Application.UseCases.Recipes;

public class GetAllRecipesUseCase(IRecipeRepository recipeRepository)
{
    private readonly IRecipeRepository _recipeRepository = recipeRepository;

    public async Task<IReadOnlyList<RecipeDto>> ExecuteAsync(
        CancellationToken cancellationToken = default
    )
    {
        var recipes = await _recipeRepository.GetAllAsync(cancellationToken);
        recipes = [];
        return [.. recipes.Select(r => r.ToDto())];
    }
}
