using CookHomie.Domain.Enums;

namespace CookHomie.Domain.Entities;

public class ShoppingItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public Priority Priority { get; set; }

    public bool IsBought { get; set; }

    public Guid? LinkedRecipeId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation property
    public Recipe? LinkedRecipe { get; set; }
}