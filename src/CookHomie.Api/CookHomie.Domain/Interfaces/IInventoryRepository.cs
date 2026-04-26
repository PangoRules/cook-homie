using CookHomie.Domain.Entities;

namespace CookHomie.Domain.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryItem> AddAsync(InventoryItem item, CancellationToken cancellationToken = default);
}
