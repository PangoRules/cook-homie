using CookHomie.Domain.Enums;

namespace CookHomie.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public Location Location { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public DateOnly? ExpiresAt { get; set; }

    public bool IsOpened { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}