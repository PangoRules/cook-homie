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

        var seededAt = new DateTime(2026, 4, 26, 0, 0, 0, DateTimeKind.Utc);

        // Shared seed Guids so RecipeIngredient rows can reference Recipe rows
        var rMac = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var rTuna = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var rEggs = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var rRice = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var rTomato = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var rTunaSand = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var rChicken = Guid.Parse("77777777-7777-7777-7777-777777777777");

        // ── InventoryItem ──────────────────────────────────────────────────────
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.Unit).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasData(
                new InventoryItem { Id = Guid.Parse("d8109ce9-f967-4f32-a4a4-5031a0df1baf"), Name = "Milk", Category = "Dairy", Location = Domain.Enums.Location.Fridge, Quantity = 1m, Unit = "liter", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("5a4b2996-ebf8-4b98-91fe-290ca2d9b2bd"), Name = "Cheddar Cheese", Category = "Dairy", Location = Domain.Enums.Location.Fridge, Quantity = 250m, Unit = "g", ExpiresAt = null, IsOpened = true, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("4f80dd83-a339-45bb-9f24-d76cc0f25818"), Name = "Spinach", Category = "Produce", Location = Domain.Enums.Location.Fridge, Quantity = 1m, Unit = "bag", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("680aef2a-9969-4346-95d0-c6fc9f426445"), Name = "Tomatoes", Category = "Produce", Location = Domain.Enums.Location.Fridge, Quantity = 6m, Unit = "units", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("fd769e8e-252a-4e14-b0ec-87a5f4baa253"), Name = "Chicken Breast", Category = "Protein", Location = Domain.Enums.Location.Freezer, Quantity = 2m, Unit = "units", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("933d0ce1-faad-4a4b-b349-1d9f60dbb0be"), Name = "Canned Tuna", Category = "Protein", Location = Domain.Enums.Location.Pantry, Quantity = 3m, Unit = "cans", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("f6f06cae-72f7-4bc2-bf16-58b7d24924fe"), Name = "Rice", Category = "Grains", Location = Domain.Enums.Location.Pantry, Quantity = 2m, Unit = "kg", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("f92ff908-beb0-4af0-90d9-5992a39cc0bc"), Name = "Pasta", Category = "Grains", Location = Domain.Enums.Location.Pantry, Quantity = 4m, Unit = "packs", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Eggs", Category = "Dairy", Location = Domain.Enums.Location.Fridge, Quantity = 12m, Unit = "units", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Butter", Category = "Dairy", Location = Domain.Enums.Location.Fridge, Quantity = 200m, Unit = "g", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Garlic", Category = "Produce", Location = Domain.Enums.Location.Pantry, Quantity = 1m, Unit = "head", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt },
                new InventoryItem { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Olive Oil", Category = "Pantry", Location = Domain.Enums.Location.Pantry, Quantity = 500m, Unit = "ml", ExpiresAt = null, IsOpened = false, Notes = null, CreatedAt = seededAt, UpdatedAt = seededAt }
            );
        });

        // ── Recipe ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Instructions).IsRequired();
            entity.Property(e => e.Tags);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasData(
                new Recipe { Id = rMac, Name = "Classic Mac & Cheese", Instructions = "Boil pasta until al dente. In a saucepan, melt butter over medium heat. Stir in flour and cook 1 minute. Gradually whisk in milk and cook until thickened. Remove from heat, stir in cheddar until melted. Combine sauce with pasta, season with salt and pepper. Serve hot.", PrepMinutes = 10, CookMinutes = 20, Tags = new[] { "comfort", "quick", "vegetarian" }, Source = null, CreatedAt = seededAt },
                new Recipe { Id = rTuna, Name = "Tuna Pasta Bake", Instructions = "Preheat oven to 200\u00b0C. Cook pasta until al dente. Drain tuna. In a large bowl, mix pasta, tuna, tomatoes, and half the cheese. Transfer to a baking dish, top with remaining cheese. Bake 20 minutes until golden and bubbly.", PrepMinutes = 15, CookMinutes = 20, Tags = new[] { "baked", "protein", "family-friendly" }, Source = null, CreatedAt = seededAt },
                new Recipe { Id = rEggs, Name = "Spinach & Egg Scramble", Instructions = "Beat eggs in a bowl with a splash of milk, salt, and pepper. Melt butter in a non-stick pan over medium-low heat. Add eggs and gently stir with a spatula. When nearly set, fold in spinach and cook 1 more minute. Serve immediately.", PrepMinutes = 5, CookMinutes = 8, Tags = new[] { "breakfast", "quick", "vegetarian" }, Source = null, CreatedAt = seededAt },
                new Recipe { Id = rRice, Name = "Chicken Fried Rice", Instructions = "Cook rice and let it cool (day-old rice works best). Scramble eggs and set aside. Heat oil in a wok over high heat. Stir-fry onion and garlic 1 minute. Add chicken and cook through. Add rice, soy sauce, and eggs. Toss everything together over high heat until well combined.", PrepMinutes = 15, CookMinutes = 15, Tags = new[] { "asian", "wok", "meal-prep" }, Source = null, CreatedAt = seededAt },
                new Recipe { Id = rTomato, Name = "Simple Tomato Pasta", Instructions = "Cook pasta in salted boiling water. Meanwhile, heat olive oil in a pan and saut\u00e9 garlic until fragrant. Add tomatoes and cook 10 minutes, crushing them lightly. Toss drained pasta with the sauce, adjust seasoning. Serve with fresh herbs if available.", PrepMinutes = 5, CookMinutes = 15, Tags = new[] { "quick", "vegan", "simple" }, Source = null, CreatedAt = seededAt },
                new Recipe { Id = rTunaSand, Name = "Classic Tuna Sandwich", Instructions = "Drain tuna and flake into a bowl. Hard boil eggs and chop. Mix tuna, eggs, cheddar, and onion. Add mayonnaise and stir until combined. Season to taste. Serve on bread with lettuce.", PrepMinutes = 15, CookMinutes = 10, Tags = new[] { "lunch", "sandwich", "protein" }, Source = null, CreatedAt = seededAt },
                new Recipe { Id = rChicken, Name = "Garlic Butter Chicken with Rice", Instructions = "Season chicken breast with salt and pepper. Melt butter in a skillet over medium-high heat. Cook chicken 5-7 minutes per side until golden and cooked through. In the last minute, add garlic and baste chicken. Rest 5 minutes, slice. Serve over steamed rice with pan juices drizzled on top.", PrepMinutes = 10, CookMinutes = 20, Tags = new[] { "protein", "garlic", "family-friendly" }, Source = null, CreatedAt = seededAt }
            );
        });

        // ── RecipeIngredient ──────────────────────────────────────────────────
        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IngredientName).IsRequired();
            entity.Property(e => e.Unit).IsRequired();
            entity.HasOne(e => e.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(
                // Mac & Cheese
                new RecipeIngredient { Id = Guid.Parse("a1010101-0101-0101-0101-010101010101"), RecipeId = rMac, IngredientName = "Pasta", Quantity = 2m, Unit = "packs", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a1010101-0101-0101-0101-010101010102"), RecipeId = rMac, IngredientName = "Cheddar Cheese", Quantity = 200m, Unit = "g", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a1010101-0101-0101-0101-010101010103"), RecipeId = rMac, IngredientName = "Butter", Quantity = 50m, Unit = "g", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a1010101-0101-0101-0101-010101010104"), RecipeId = rMac, IngredientName = "Milk", Quantity = 1m, Unit = "liter", IsOptional = false },
                // Tuna Pasta Bake
                new RecipeIngredient { Id = Guid.Parse("a2020202-0202-0202-0202-020202020201"), RecipeId = rTuna, IngredientName = "Pasta", Quantity = 2m, Unit = "packs", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a2020202-0202-0202-0202-020202020202"), RecipeId = rTuna, IngredientName = "Canned Tuna", Quantity = 2m, Unit = "cans", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a2020202-0202-0202-0202-020202020203"), RecipeId = rTuna, IngredientName = "Cheddar Cheese", Quantity = 150m, Unit = "g", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a2020202-0202-0202-0202-020202020204"), RecipeId = rTuna, IngredientName = "Tomatoes", Quantity = 4m, Unit = "units", IsOptional = false },
                // Spinach & Egg Scramble
                new RecipeIngredient { Id = Guid.Parse("a3030303-0303-0303-0303-030303030301"), RecipeId = rEggs, IngredientName = "Eggs", Quantity = 4m, Unit = "units", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a3030303-0303-0303-0303-030303030302"), RecipeId = rEggs, IngredientName = "Spinach", Quantity = 1m, Unit = "bag", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a3030303-0303-0303-0303-030303030303"), RecipeId = rEggs, IngredientName = "Butter", Quantity = 30m, Unit = "g", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a3030303-0303-0303-0303-030303030304"), RecipeId = rEggs, IngredientName = "Onion", Quantity = 1m, Unit = "units", IsOptional = true },
                // Chicken Fried Rice
                new RecipeIngredient { Id = Guid.Parse("a4040404-0404-0404-0404-040404040401"), RecipeId = rRice, IngredientName = "Rice", Quantity = 1m, Unit = "kg", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a4040404-0404-0404-0404-040404040402"), RecipeId = rRice, IngredientName = "Chicken Breast", Quantity = 2m, Unit = "units", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a4040404-0404-0404-0404-040404040403"), RecipeId = rRice, IngredientName = "Eggs", Quantity = 3m, Unit = "units", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a4040404-0404-0404-0404-040404040404"), RecipeId = rRice, IngredientName = "Onion", Quantity = 1m, Unit = "units", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a4040404-0404-0404-0404-040404040405"), RecipeId = rRice, IngredientName = "Garlic", Quantity = 2m, Unit = "cloves", IsOptional = false },
                // Simple Tomato Pasta
                new RecipeIngredient { Id = Guid.Parse("a5050505-0505-0505-0505-050505050501"), RecipeId = rTomato, IngredientName = "Pasta", Quantity = 2m, Unit = "packs", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a5050505-0505-0505-0505-050505050502"), RecipeId = rTomato, IngredientName = "Tomatoes", Quantity = 6m, Unit = "units", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a5050505-0505-0505-0505-050505050503"), RecipeId = rTomato, IngredientName = "Garlic", Quantity = 4m, Unit = "cloves", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a5050505-0505-0505-0505-050505050504"), RecipeId = rTomato, IngredientName = "Olive Oil", Quantity = 3m, Unit = "tbsp", IsOptional = false },
                // Classic Tuna Sandwich
                new RecipeIngredient { Id = Guid.Parse("a6060606-0606-0606-0606-060606060601"), RecipeId = rTunaSand, IngredientName = "Canned Tuna", Quantity = 2m, Unit = "cans", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a6060606-0606-0606-0606-060606060602"), RecipeId = rTunaSand, IngredientName = "Eggs", Quantity = 2m, Unit = "units", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a6060606-0606-0606-0606-060606060603"), RecipeId = rTunaSand, IngredientName = "Cheddar Cheese", Quantity = 50m, Unit = "g", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a6060606-0606-0606-0606-060606060604"), RecipeId = rTunaSand, IngredientName = "Onion", Quantity = 1m, Unit = "units", IsOptional = true },
                // Garlic Butter Chicken
                new RecipeIngredient { Id = Guid.Parse("a7070707-0707-0707-0707-070707070701"), RecipeId = rChicken, IngredientName = "Chicken Breast", Quantity = 2m, Unit = "units", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a7070707-0707-0707-0707-070707070702"), RecipeId = rChicken, IngredientName = "Butter", Quantity = 60m, Unit = "g", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a7070707-0707-0707-0707-070707070703"), RecipeId = rChicken, IngredientName = "Garlic", Quantity = 3m, Unit = "cloves", IsOptional = false },
                new RecipeIngredient { Id = Guid.Parse("a7070707-0707-0707-0707-070707070704"), RecipeId = rChicken, IngredientName = "Rice", Quantity = 1m, Unit = "kg", IsOptional = false }
            );
        });

        // ── ShoppingItem ──────────────────────────────────────────────────────
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