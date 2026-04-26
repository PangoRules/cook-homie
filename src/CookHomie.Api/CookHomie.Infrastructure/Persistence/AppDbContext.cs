using CookHomie.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // InventoryItem
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.Unit).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            var seededAt = new DateTime(2026, 4, 26, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
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
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
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
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
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
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
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
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
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
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
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
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
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
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
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
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                });
        });

        // Recipe
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Instructions).IsRequired();
            entity.Property(e => e.Tags);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // RecipeIngredient
        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IngredientName).IsRequired();
            entity.Property(e => e.Unit).IsRequired();
            entity.HasOne(e => e.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ShoppingItem
        modelBuilder.Entity<ShoppingItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasOne(e => e.LinkedRecipe)
                .WithMany()
                .HasForeignKey(e => e.LinkedRecipeId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        });
    }
}
