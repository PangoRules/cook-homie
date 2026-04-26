namespace CookHomie.Domain.Entities;

public class RecipeIngredient
{
    public Guid Id { get; set; }

    public Guid RecipeId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public bool IsOptional { get; set; }

    // Navigation property
    public Recipe? Recipe { get; set; }
}