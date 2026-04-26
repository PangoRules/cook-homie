namespace CookHomie.Domain.Entities;

public class Recipe
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public int PrepMinutes { get; set; }

    public int CookMinutes { get; set; }

    public string[] Tags { get; set; } = [];

    public string? Source { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation property
    public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
}