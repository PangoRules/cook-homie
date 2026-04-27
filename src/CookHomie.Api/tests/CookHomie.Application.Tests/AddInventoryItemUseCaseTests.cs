using CookHomie.Application.DTOs;
using CookHomie.Application.UseCases.Inventory;
using CookHomie.Domain.Entities;
using CookHomie.Domain.Enums;
using CookHomie.Domain.Interfaces;

namespace CookHomie.Application.Tests;

public class AddInventoryItemUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithBlankName_ThrowsArgumentException()
    {
        var repository = new FakeInventoryRepository();
        var useCase = new AddInventoryItemUseCase(repository);
        var request = new AddInventoryItemRequest
        {
            Name = "   ",
            Category = "Dairy",
            Quantity = 1,
            Location = "Fridge",
            Unit = "liter"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonPositiveQuantity_ThrowsArgumentOutOfRangeException()
    {
        var repository = new FakeInventoryRepository();
        var useCase = new AddInventoryItemUseCase(repository);
        var request = new AddInventoryItemRequest
        {
            Name = "Milk",
            Category = "Dairy",
            Quantity = 0,
            Location = "Fridge",
            Unit = "liter"
        };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidLocation_ThrowsArgumentException()
    {
        var repository = new FakeInventoryRepository();
        var useCase = new AddInventoryItemUseCase(repository);
        var request = new AddInventoryItemRequest
        {
            Name = "Milk",
            Category = "Dairy",
            Quantity = 2,
            Location = "Cellar",
            Unit = "liter"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ReturnsCreatedItem()
    {
        var repository = new FakeInventoryRepository();
        var useCase = new AddInventoryItemUseCase(repository);
        var request = new AddInventoryItemRequest
        {
            Name = "Milk",
            Category = "Dairy",
            Quantity = 2,
            Location = "fRidGe",
            Unit = "liter"
        };

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal("Milk", result.Name);
        Assert.Equal("Dairy", result.Category);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(Location.Fridge.ToString(), result.Location);
        Assert.Equal("liter", result.Unit);
        Assert.False(result.IsOpened);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(repository.LastAdded);
        Assert.Equal(Location.Fridge, repository.LastAdded!.Location);
        Assert.Equal("Dairy", repository.LastAdded.Category);
        Assert.Equal("liter", repository.LastAdded.Unit);
    }

    [Fact]
    public async Task ExecuteAsync_WithBlankCategory_ThrowsArgumentException()
    {
        var repository = new FakeInventoryRepository();
        var useCase = new AddInventoryItemUseCase(repository);
        var request = new AddInventoryItemRequest
        {
            Name = "Milk",
            Category = "   ",
            Quantity = 1,
            Location = "Fridge",
            Unit = "liter"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WithBlankUnit_ThrowsArgumentException()
    {
        var repository = new FakeInventoryRepository();
        var useCase = new AddInventoryItemUseCase(repository);
        var request = new AddInventoryItemRequest
        {
            Name = "Milk",
            Category = "Dairy",
            Quantity = 1,
            Location = "Fridge",
            Unit = "   "
        };

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(request));
    }

    private sealed class FakeInventoryRepository : IInventoryRepository
    {
        public InventoryItem? LastAdded { get; private set; }

        public Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<InventoryItem>>([]);
        }

        public Task<InventoryItem> AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
        {
            LastAdded = item;
            return Task.FromResult(item);
        }
    }
}
