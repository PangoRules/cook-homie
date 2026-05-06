using CookHomie.Application.DTOs;
using CookHomie.Application.Mappings;
using CookHomie.Domain.Entities;
using CookHomie.Domain.Interfaces;

namespace CookHomie.Application.UseCases.Recipes;

public class CreateRecipeUseCase(IRecipeRepository recipeRepository)
{
    private readonly IRecipeRepository _recipeRepository = recipeRepository;

    public async Task<RecipeDto> ExecuteAsync(
        UpsertRecipeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Instructions))
        {
            throw new ArgumentException("Instructions are required.", nameof(request));
        }

        if (request.PrepMinutes < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                "PrepMinutes cannot be negative."
            );
        }

        if (request.CookMinutes < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                "CookMinutes cannot be negative."
            );
        }

        if (request.Ingredients is null || request.Ingredients.Count == 0)
        {
            throw new ArgumentException("At least one ingredient is required.", nameof(request));
        }

        var now = DateTime.UtcNow;
        var recipeId = Guid.NewGuid();
        var ingredients = request
            .Ingredients.Select(ing =>
            {
                if (string.IsNullOrWhiteSpace(ing.IngredientName))
                {
                    throw new ArgumentException(
                        "Each ingredient must have a name.",
                        nameof(request)
                    );
                }

                if (string.IsNullOrWhiteSpace(ing.Unit))
                {
                    throw new ArgumentException(
                        "Each ingredient must have a unit.",
                        nameof(request)
                    );
                }

                if (ing.Quantity <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(request),
                        "Ingredient quantity must be greater than zero."
                    );
                }

                return new RecipeIngredient
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipeId,
                    IngredientName = ing.IngredientName.Trim(),
                    Quantity = ing.Quantity,
                    Unit = ing.Unit.Trim(),
                    IsOptional = ing.IsOptional,
                };
            })
            .ToList();

        var recipe = new Recipe
        {
            Id = recipeId,
            Name = request.Name.Trim(),
            Instructions = request.Instructions.Trim(),
            PrepMinutes = request.PrepMinutes,
            CookMinutes = request.CookMinutes,
            Tags =
                request
                    .Tags?.Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToArray()
                ?? [],
            Source = string.IsNullOrWhiteSpace(request.Source) ? null : request.Source.Trim(),
            CreatedAt = now,
            Ingredients = ingredients,
        };

        var created = await _recipeRepository.AddAsync(recipe, cancellationToken);
        return created.ToDto();
    }
}
