using CookHomie.Domain.Entities;

namespace CookHomie.Domain.Interfaces;

public interface IRecipeRepository
{
    Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Recipe> AddAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task<Recipe?> UpdateAsync(Recipe recipe, IReadOnlyList<RecipeIngredient>? newIngredients = null, CancellationToken cancellationToken = default);
}