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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}