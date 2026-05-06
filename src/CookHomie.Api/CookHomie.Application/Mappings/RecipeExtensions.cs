using CookHomie.Application.DTOs;
using CookHomie.Domain.Entities;

namespace CookHomie.Application.Mappings;

public static class RecipeExtensions
{
    public static RecipeDto ToDto(this Recipe recipe)
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
            Ingredients =
            [
                .. recipe
                    .Ingredients.OrderBy(i => i.IngredientName)
                    .Select(i => new RecipeIngredientDto
                    {
                        Id = i.Id,
                        IngredientName = i.IngredientName,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                        IsOptional = i.IsOptional,
                    }),
            ],
        };
    }
}

