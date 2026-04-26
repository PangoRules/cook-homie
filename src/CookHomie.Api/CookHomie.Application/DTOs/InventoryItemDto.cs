namespace CookHomie.Application.DTOs;

public class InventoryItemDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Location { get; set; } = string.Empty;
}
