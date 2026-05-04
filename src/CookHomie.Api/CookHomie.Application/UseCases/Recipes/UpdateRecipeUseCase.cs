using CookHomie.Application.DTOs;
using CookHomie.Domain.Entities;
using CookHomie.Domain.Interfaces;

namespace CookHomie.Application.UseCases.Recipes;

public class UpdateRecipeUseCase
{
    private readonly IRecipeRepository _recipeRepository;

    public UpdateRecipeUseCase(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeDto?> ExecuteAsync(Guid id, UpsertRecipeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _recipeRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = string.IsNullOrWhiteSpace(request.Name) ? existing.Name : request.Name.Trim();
        existing.Instructions = string.IsNullOrWhiteSpace(request.Instructions) ? existing.Instructions : request.Instructions.Trim();
        existing.PrepMinutes = request.PrepMinutes < 0 ? existing.PrepMinutes : request.PrepMinutes;
        existing.CookMinutes = request.CookMinutes < 0 ? existing.CookMinutes : request.CookMinutes;
        existing.Tags = request.Tags?
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToArray() ?? existing.Tags;
        existing.Source = string.IsNullOrWhiteSpace(request.Source) ? existing.Source : request.Source.Trim();

        IReadOnlyList<RecipeIngredient>? newIngredients = null;

        if (request.Ingredients is not null)
        {
            foreach (var ing in request.Ingredients)
            {
                if (string.IsNullOrWhiteSpace(ing.IngredientName))
                {
                    throw new ArgumentException("Each ingredient must have a name.", nameof(request));
                }

                if (string.IsNullOrWhiteSpace(ing.Unit))
                {
                    throw new ArgumentException("Each ingredient must have a unit.", nameof(request));
                }

                if (ing.Quantity <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(request), "Ingredient quantity must be greater than zero.");
                }
            }

            // Repository handles ingredient replacement directly
            newIngredients = request.Ingredients
                .Select(ing => new RecipeIngredient
                {
                    Id = Guid.NewGuid(),
                    RecipeId = existing.Id,
                    IngredientName = ing.IngredientName.Trim(),
                    Quantity = ing.Quantity,
                    Unit = ing.Unit.Trim(),
                    IsOptional = ing.IsOptional
                })
                .ToList();
        }

        var updated = await _recipeRepository.UpdateAsync(existing, request.Ingredients is not null ? newIngredients : null, cancellationToken);
        return MapToDto(updated);
    }

    private static RecipeDto MapToDto(Recipe recipe)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Instructions = recipe.Instructions,
            PrepMinutes = recipe.PrepMinutes,
            CookMinutes = recipe.CookMinutes,
            Tags = recipe.Tags,
            Source = recipe.Source,
            CreatedAt = recipe.CreatedAt,
            Ingredients = recipe.Ingredients
                .OrderBy(i => i.IngredientName)
                .Select(i => new RecipeIngredientDto
                {
                    Id = i.Id,
                    IngredientName = i.IngredientName,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    IsOptional = i.IsOptional
                })
                .ToList()
        };
    }
}