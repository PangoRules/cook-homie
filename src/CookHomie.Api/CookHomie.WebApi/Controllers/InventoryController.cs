using CookHomie.Domain.Entities;
using CookHomie.Domain.Enums;
using CookHomie.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CookHomie.WebApi.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryController(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventoryItemResponse>>> Get(CancellationToken cancellationToken)
    {
        var items = await _inventoryRepository.GetAllAsync(cancellationToken);
        return Ok(items.Select(MapToResponse).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItemResponse>> Post([FromBody] CreateInventoryItemRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Category) ||
            string.IsNullOrWhiteSpace(request.Location) ||
            string.IsNullOrWhiteSpace(request.Unit) ||
            request.Quantity <= 0)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid inventory payload",
                detail: "Name, category, location, unit must be provided and quantity must be greater than zero.");
        }

        if (!Enum.TryParse<Location>(request.Location, true, out var parsedLocation))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid location",
                detail: "Location must be one of Pantry, Fridge, Freezer, or Spices.");
        }

        var now = DateTime.UtcNow;
        var created = await _inventoryRepository.AddAsync(new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Location = parsedLocation,
            Quantity = request.Quantity,
            Unit = request.Unit.Trim(),
            ExpiresAt = request.ExpiresAt,
            IsOpened = request.IsOpened,
            Notes = request.Notes,
            CreatedAt = now,
            UpdatedAt = now
        }, cancellationToken);

        return Ok(MapToResponse(created));
    }

    private static InventoryItemResponse MapToResponse(InventoryItem item)
    {
        return new InventoryItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category,
            Location = item.Location.ToString(),
            Quantity = item.Quantity,
            Unit = item.Unit,
            ExpiresAt = item.ExpiresAt,
            IsOpened = item.IsOpened,
            Notes = item.Notes
        };
    }

    public sealed class CreateInventoryItemRequest
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

    public sealed class InventoryItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateOnly? ExpiresAt { get; set; }
        public bool IsOpened { get; set; }
        public string? Notes { get; set; }
    }
}
