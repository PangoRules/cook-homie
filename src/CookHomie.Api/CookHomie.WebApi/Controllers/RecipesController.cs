using CookHomie.Application.DTOs;
using CookHomie.Application.UseCases.Recipes;
using CookHomie.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CookHomie.WebApi.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly CreateRecipeUseCase _createRecipe;
    private readonly UpdateRecipeUseCase _updateRecipe;
    private readonly GetMissingIngredientsUseCase _getMissingIngredients;

    public RecipesController(
        IRecipeRepository recipeRepository,
        CreateRecipeUseCase createRecipe,
        UpdateRecipeUseCase updateRecipe,
        GetMissingIngredientsUseCase getMissingIngredients)
    {
        _recipeRepository = recipeRepository;
        _createRecipe = createRecipe;
        _updateRecipe = updateRecipe;
        _getMissingIngredients = getMissingIngredients;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RecipeDto>>> Get(CancellationToken cancellationToken)
    {
        var recipes = await _recipeRepository.GetAllAsync(cancellationToken);
        return Ok(recipes.Select(MapToDto).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RecipeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id, cancellationToken);
        if (recipe is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Recipe not found",
                detail: $"No recipe found with ID {id}."
            );
        }

        return Ok(MapToDto(recipe));
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> Post(
        [FromBody] UpsertRecipeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _createRecipe.ExecuteAsync(request, cancellationToken);
            return Created($"/api/recipes/{result.Id}", result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid recipe payload",
                detail: ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid recipe payload",
                detail: ex.Message);
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<RecipeDto>> Patch(
        Guid id,
        [FromBody] UpsertRecipeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _updateRecipe.ExecuteAsync(id, request, cancellationToken);
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
                detail: ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid recipe payload",
                detail: ex.Message);
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

    [HttpGet("{id:guid}/missing")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetMissingIngredients(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getMissingIngredients.ExecuteAsync(id, cancellationToken);
        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Recipe not found",
                detail: $"No recipe found with ID {id}."
            );
        }

        return Ok(result.ToList());
    }

    private static RecipeDto MapToDto(Domain.Entities.Recipe recipe)
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