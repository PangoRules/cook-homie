using CookHomie.Application.DTOs;
using CookHomie.Domain.Entities;
using CookHomie.Domain.Enums;
using CookHomie.Domain.Interfaces;

namespace CookHomie.Application.UseCases.Inventory;

public class AddInventoryItemUseCase(IInventoryRepository inventoryRepository)
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;

    public async Task<InventoryItemDto> ExecuteAsync(
        AddInventoryItemRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ArgumentException("Category is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Unit))
        {
            throw new ArgumentException("Unit is required.", nameof(request));
        }

        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Quantity),
                "Quantity must be greater than zero."
            );
        }

        if (!Enum.TryParse(request.Location, true, out Location parsedLocation))
        {
            throw new ArgumentException("Location is invalid.", nameof(request.Location));
        }

        var now = DateTime.UtcNow;
        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Location = parsedLocation,
            Quantity = request.Quantity,
            Unit = request.Unit.Trim(),
            ExpiresAt = request.ExpiresAt,
            IsOpened = request.IsOpened,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
        };

        var createdItem = await _inventoryRepository.AddAsync(item, cancellationToken);

        return new InventoryItemDto
        {
            Id = createdItem.Id,
            Name = createdItem.Name,
            Category = createdItem.Category,
            Location = createdItem.Location.ToString(),
            Quantity = createdItem.Quantity,
            Unit = createdItem.Unit,
            ExpiresAt = createdItem.ExpiresAt,
            IsOpened = createdItem.IsOpened,
            Notes = createdItem.Notes,
        };
    }
}
