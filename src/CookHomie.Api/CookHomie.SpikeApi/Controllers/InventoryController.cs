using Microsoft.AspNetCore.Mvc;

namespace CookHomie.SpikeApi.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private static readonly List<InventoryItemResponse> Items = [];
    private static readonly object Sync = new();

    [HttpGet]
    public ActionResult<IReadOnlyList<InventoryItemResponse>> Get()
    {
        lock (Sync)
        {
            return Ok(Items.ToList());
        }
    }

    [HttpPost]
    public ActionResult<InventoryItemResponse> Post([FromBody] AddInventoryItemRequest request)
    {
        var created = new InventoryItemResponse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Category = request.Category,
            Location = request.Location,
            Quantity = request.Quantity,
            Unit = request.Unit,
            ExpiresAt = request.ExpiresAt,
            IsOpened = request.IsOpened,
            Notes = request.Notes
        };

        lock (Sync)
        {
            Items.Insert(0, created);
        }

        return Ok(created);
    }

    public sealed class AddInventoryItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string? ExpiresAt { get; set; }
        public bool IsOpened { get; set; }
        public string? Notes { get; set; }
    }

    public sealed class InventoryItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string? ExpiresAt { get; set; }
        public bool IsOpened { get; set; }
        public string? Notes { get; set; }
    }
}
