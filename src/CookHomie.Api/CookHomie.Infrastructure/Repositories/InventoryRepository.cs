using CookHomie.Domain.Entities;
using CookHomie.Domain.Interfaces;
using CookHomie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Repositories;

public class InventoryRepository(AppDbContext context) : IInventoryRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .InventoryItems.OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetByNamesAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken = default
    )
    {
        var nameSet = names
            .Select(n => n.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return await _context
            .InventoryItems.Where(i => nameSet.Contains(i.Name.ToLower()))
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryItem> AddAsync(
        InventoryItem item,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(item);
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }
}
