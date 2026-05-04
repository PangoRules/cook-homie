using CookHomie.Infrastructure.Persistence;
using CookHomie.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Tests;

public class InventoryRepositoryTests
{
    [Fact]
    public async Task EnsureCreated_SeedsInventoryItems()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        await using var db = new AppDbContext(options);

        await db.Database.EnsureCreatedAsync();

        var count = await db.InventoryItems.CountAsync();
        Assert.Equal(12, count); // 8 baseline + 4 added for recipe seed coverage
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
