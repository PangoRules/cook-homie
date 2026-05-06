using CookHomie.Application.DTOs;
using CookHomie.Application.UseCases.Recipes;
using Microsoft.AspNetCore.Mvc;

namespace CookHomie.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController(
    CreateRecipeUseCase createRecipe,
    UpdateRecipeUseCase updateRecipe,
    GetRecipesUseCase getRecipes
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RecipeDto>>> Get(
        CancellationToken cancellationToken
    )
    {
        var recipes = await getRecipes.GetAllAsync(cancellationToken);
        return recipes.Count > 0 ? Ok(recipes) : StatusCode(StatusCodes.Status204NoContent);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RecipeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var recipe = await getRecipes.GetByIdAsync(id, cancellationToken);
        if (recipe is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Recipe not found",
                detail: $"No recipe found with ID {id}."
            );
        }

        return Ok(recipe);
    }

    [HttpGet("{id:guid}/stock-check")]
    public async Task<ActionResult<RecipeStockCheckDto>> GetStockCheck(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await getRecipes.GetStockCheckAsync(id, cancellationToken);
        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Recipe not found",
                detail: $"No recipe found with ID {id}."
            );
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Post(
        [FromBody] UpsertRecipeRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await createRecipe.ExecuteAsync(request, cancellationToken);
            return Created($"/api/recipes/{result.Id}", result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid recipe payload",
                detail: ex.Message
            );
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid recipe payload",
                detail: ex.Message
            );
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<RecipeDto>> Patch(
        Guid id,
        [FromBody] UpsertRecipeRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await updateRecipe.ExecuteAsync(id, request, cancellationToken);
            if (result is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Recipe not found",
                    detail: $"No recipe found with ID {id}."
                );
            }

            return Ok(result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid recipe payload",
                detail: ex.Message
            );
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid recipe payload",
                detail: ex.Message
            );
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal error",
                detail: ex.Message
            );
        }
    }
}
