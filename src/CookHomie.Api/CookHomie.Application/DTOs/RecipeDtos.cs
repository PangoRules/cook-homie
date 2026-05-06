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

public class RecipeStockCheckDto
{
    public Guid RecipeId { get; set; }
    public bool CanCook { get; set; }
    public int MissingCount { get; set; }
    public int InsufficientCount { get; set; }
    public int GoodCount { get; set; }
    public List<RecipeStockItemDto> Items { get; set; } = [];
}

public class RecipeStockItemDto
{
    public string IngredientName { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
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
