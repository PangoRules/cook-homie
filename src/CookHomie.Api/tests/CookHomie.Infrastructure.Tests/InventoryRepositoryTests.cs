using CookHomie.Infrastructure.Persistence;
using CookHomie.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Tests;

public class InventoryRepositoryTests
{
    [Fact]
    public async Task EnsureCreated_SeedsTwoItemsPerCategory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        await using var db = new AppDbContext(options);

        await db.Database.EnsureCreatedAsync();

        var groupedCounts = await db.InventoryItems
            .GroupBy(i => i.Category)
            .Select(group => new { Category = group.Key, Count = group.Count() })
            .ToListAsync();

        Assert.NotEmpty(groupedCounts);
        Assert.All(groupedCounts, group => Assert.Equal(2, group.Count));
    }

    [Fact]
    public async Task AddAsync_PersistsInventoryItem()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        await using var db = new AppDbContext(options);
        var repo = new InventoryRepository(db);
        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = "Eggs",
            Category = "Dairy",
            Location = Location.Fridge,
            Quantity = 12m,
            Unit = "units",
            IsOpened = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        await repo.AddAsync(item);

        // Assert
        var stored = await db.InventoryItems.FirstOrDefaultAsync(i => i.Id == item.Id);
        Assert.NotNull(stored);
        Assert.Equal("Eggs", stored.Name);
        Assert.Equal(Location.Fridge, stored.Location);
    }
}
