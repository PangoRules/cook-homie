using CookHomie.Domain.Entities;
using CookHomie.Domain.Interfaces;
using CookHomie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Repositories;

public class RecipeRepository(AppDbContext context) : IRecipeRepository
{
    public async Task<IReadOnlyList<Recipe>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Recipes.Include(r => r.Ingredients)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context
            .Recipes.Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Recipe> AddAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recipe);
        context.Recipes.Add(recipe);
        await context.SaveChangesAsync(cancellationToken);
        return recipe;
    }

    public async Task<Recipe?> UpdateAsync(
        Recipe recipe,
        IReadOnlyList<RecipeIngredient>? newIngredients = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(recipe);

        // Re-query to get a tracked copy — prevents "different tracked entity with same key" issues
        var tracked = await context
            .Recipes.Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == recipe.Id, cancellationToken);

        if (tracked is null)
        {
            return null;
        }

        // Apply scalar field changes
        tracked.Name = recipe.Name;
        tracked.Instructions = recipe.Instructions;
        tracked.PrepMinutes = recipe.PrepMinutes;
        tracked.CookMinutes = recipe.CookMinutes;
        tracked.Tags = recipe.Tags;
        tracked.Source = recipe.Source;

        if (newIngredients is not null)
        {
            // Remove old ingredients directly from the change tracker without
            // touching the navigation property (avoids InMemory provider edge cases
            // with ICollection<T>.Clear() on proxied collections)
            var oldIngredients = await context
                .RecipeIngredients.Where(i => i.RecipeId == tracked.Id)
                .ToListAsync(cancellationToken);

            context.RecipeIngredients.RemoveRange(oldIngredients);

            // Add new pre-built ingredients
            foreach (var ing in newIngredients)
            {
                context.RecipeIngredients.Add(
                    new RecipeIngredient
                    {
                        Id = Guid.NewGuid(),
                        RecipeId = tracked.Id,
                        IngredientName = ing.IngredientName,
                        Quantity = ing.Quantity,
                        Unit = ing.Unit,
                        IsOptional = ing.IsOptional,
                    }
                );
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        // Reload to get DB-generated values (timestamps, etc.)
        await context.Entry(tracked).Collection(r => r.Ingredients).LoadAsync(cancellationToken);
        return tracked;
    }
}
