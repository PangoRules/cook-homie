using CookHomie.Domain.Entities;
using CookHomie.Domain.Interfaces;
using CookHomie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _context;

    public InventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem> AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }
}