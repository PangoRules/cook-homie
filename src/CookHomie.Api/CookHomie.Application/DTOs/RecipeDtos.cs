namespace CookHomie.Application.DTOs;

public class RecipeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int PrepMinutes { get; set; }
    public int CookMinutes { get; set; }
    public string[] Tags { get; set; } = [];
    public string? Source { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RecipeIngredientDto> Ingredients { get; set; } = [];
}

public class RecipeIngredientDto
{
    public Guid Id { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsOptional { get; set; }
}

public class UpsertRecipeRequest
{
    public string Name { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int PrepMinutes { get; set; }
    public int CookMinutes { get; set; }
    public string[] Tags { get; set; } = [];
    public string? Source { get; set; }
    public List<UpsertRecipeIngredientRequest> Ingredients { get; set; } = [];
}

public class UpsertRecipeIngredientRequest
{
    public Guid? Id { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsOptional { get; set; }
}