namespace CookHomie.Application.DTOs;

public class AddInventoryItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateOnly? ExpiresAt { get; set; }
    public bool IsOpened { get; set; }
    public string? Notes { get; set; }
}
