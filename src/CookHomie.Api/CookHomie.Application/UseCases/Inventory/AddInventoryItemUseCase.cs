using CookHomie.Application.DTOs;
using CookHomie.Domain.Entities;
using CookHomie.Domain.Enums;
using CookHomie.Domain.Interfaces;

namespace CookHomie.Application.UseCases.Inventory;

public class AddInventoryItemUseCase
{
    private readonly IInventoryRepository _inventoryRepository;

    public AddInventoryItemUseCase(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<InventoryItemDto> ExecuteAsync(AddInventoryItemRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request));
        }

        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Quantity), "Quantity must be greater than zero.");
        }

        if (!Enum.TryParse(request.Location, true, out Location parsedLocation))
        {
            throw new ArgumentException("Location is invalid.", nameof(request.Location));
        }

        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Quantity = request.Quantity,
            Location = parsedLocation,
            Category = string.Empty,
            Unit = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdItem = await _inventoryRepository.AddAsync(item, cancellationToken);

        return new InventoryItemDto
        {
            Id = createdItem.Id,
            Name = createdItem.Name,
            Quantity = createdItem.Quantity,
            Location = createdItem.Location.ToString()
        };
    }
}
