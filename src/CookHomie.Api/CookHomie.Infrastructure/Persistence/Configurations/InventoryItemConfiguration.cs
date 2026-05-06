using CookHomie.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookHomie.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    private static readonly DateTime SeededAt = new(2026, 4, 26, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Category).IsRequired();
        builder.Property(e => e.Unit).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();

        builder.HasData(
            new InventoryItem
            {
                Id = Guid.Parse("d8109ce9-f967-4f32-a4a4-5031a0df1baf"),
                Name = "Milk",
                Category = "Dairy",
                Location = Domain.Enums.Location.Fridge,
                Quantity = 1m,
                Unit = "liter",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("5a4b2996-ebf8-4b98-91fe-290ca2d9b2bd"),
                Name = "Cheddar Cheese",
                Category = "Dairy",
                Location = Domain.Enums.Location.Fridge,
                Quantity = 250m,
                Unit = "g",
                ExpiresAt = null,
                IsOpened = true,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("4f80dd83-a339-45bb-9f24-d76cc0f25818"),
                Name = "Spinach",
                Category = "Produce",
                Location = Domain.Enums.Location.Fridge,
                Quantity = 1m,
                Unit = "bag",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("680aef2a-9969-4346-95d0-c6fc9f426445"),
                Name = "Tomatoes",
                Category = "Produce",
                Location = Domain.Enums.Location.Fridge,
                Quantity = 6m,
                Unit = "units",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("fd769e8e-252a-4e14-b0ec-87a5f4baa253"),
                Name = "Chicken Breast",
                Category = "Protein",
                Location = Domain.Enums.Location.Freezer,
                Quantity = 2m,
                Unit = "units",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("933d0ce1-faad-4a4b-b349-1d9f60dbb0be"),
                Name = "Canned Tuna",
                Category = "Protein",
                Location = Domain.Enums.Location.Pantry,
                Quantity = 3m,
                Unit = "cans",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("f6f06cae-72f7-4bc2-bf16-58b7d24924fe"),
                Name = "Rice",
                Category = "Grains",
                Location = Domain.Enums.Location.Pantry,
                Quantity = 2m,
                Unit = "kg",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("f92ff908-beb0-4af0-90d9-5992a39cc0bc"),
                Name = "Pasta",
                Category = "Grains",
                Location = Domain.Enums.Location.Pantry,
                Quantity = 4m,
                Unit = "packs",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Eggs",
                Category = "Dairy",
                Location = Domain.Enums.Location.Fridge,
                Quantity = 12m,
                Unit = "units",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Butter",
                Category = "Dairy",
                Location = Domain.Enums.Location.Fridge,
                Quantity = 200m,
                Unit = "g",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Garlic",
                Category = "Produce",
                Location = Domain.Enums.Location.Pantry,
                Quantity = 1m,
                Unit = "head",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            },
            new InventoryItem
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Olive Oil",
                Category = "Pantry",
                Location = Domain.Enums.Location.Pantry,
                Quantity = 500m,
                Unit = "ml",
                ExpiresAt = null,
                IsOpened = false,
                Notes = null,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
            }
        );
    }
}

