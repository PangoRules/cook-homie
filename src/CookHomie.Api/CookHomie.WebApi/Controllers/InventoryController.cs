using CookHomie.Domain.Entities;
using CookHomie.Domain.Enums;
using CookHomie.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CookHomie.Application.DTOs;
using CookHomie.Application.UseCases.Inventory;

namespace CookHomie.WebApi.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly AddInventoryItemUseCase _addInventoryItem;

    public InventoryController(
        IInventoryRepository inventoryRepository,
        AddInventoryItemUseCase addInventoryItem)
    {
        _inventoryRepository = inventoryRepository;
        _addInventoryItem = addInventoryItem;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventoryItemResponse>>> Get(CancellationToken cancellationToken)
    {
        var items = await _inventoryRepository.GetAllAsync(cancellationToken);
        return Ok(items.Select(MapToResponse).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItemDto>> Post(
        [FromBody] AddInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _addInventoryItem.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid inventory payload",
                detail: ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid inventory payload",
                detail: ex.Message);
        }
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
