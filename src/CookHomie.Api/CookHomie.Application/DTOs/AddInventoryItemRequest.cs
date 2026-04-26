namespace CookHomie.Application.DTOs;

public class AddInventoryItemRequest
{
    public string Name { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string Location { get; set; } = string.Empty;
}
