using CookHomie.Domain.Entities;

namespace CookHomie.Domain.Interfaces;

public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryItem>> GetByNamesAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken = default
    );
    Task<InventoryItem> AddAsync(InventoryItem item, CancellationToken cancellationToken = default);
}

