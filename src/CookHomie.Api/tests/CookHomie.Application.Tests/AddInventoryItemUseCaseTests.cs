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
            Quantity = 1,
            Location = "Fridge"
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
            Quantity = 0,
            Location = "Fridge"
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
            Quantity = 2,
            Location = "Cellar"
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
            Quantity = 2,
            Location = "fRidGe"
        };

        var result = await useCase.ExecuteAsync(request);

        Assert.Equal("Milk", result.Name);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(Location.Fridge.ToString(), result.Location);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(repository.LastAdded);
        Assert.Equal(Location.Fridge, repository.LastAdded!.Location);
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
