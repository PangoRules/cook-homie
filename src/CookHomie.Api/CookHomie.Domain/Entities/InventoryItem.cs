using CookHomie.Domain.Enums;

namespace CookHomie.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public Location Location { get; set; }
}
