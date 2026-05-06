using CookHomie.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CookHomie.Infrastructure.Persistence.Configurations.RecipeConfiguration;

namespace CookHomie.Infrastructure.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IngredientName).IsRequired();
        builder.Property(e => e.Unit).IsRequired();
        builder
            .HasOne(e => e.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(e => e.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            // Mac & Cheese
            new RecipeIngredient
            {
                Id = Guid.Parse("a1010101-0101-0101-0101-010101010101"),
                RecipeId = RMac,
                IngredientName = "Pasta",
                Quantity = 2m,
                Unit = "packs",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a1010101-0101-0101-0101-010101010102"),
                RecipeId = RMac,
                IngredientName = "Cheddar Cheese",
                Quantity = 200m,
                Unit = "g",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a1010101-0101-0101-0101-010101010103"),
                RecipeId = RMac,
                IngredientName = "Butter",
                Quantity = 50m,
                Unit = "g",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a1010101-0101-0101-0101-010101010104"),
                RecipeId = RMac,
                IngredientName = "Milk",
                Quantity = 1m,
                Unit = "liter",
                IsOptional = false,
            },
            // Tuna Pasta Bake
            new RecipeIngredient
            {
                Id = Guid.Parse("a2020202-0202-0202-0202-020202020201"),
                RecipeId = RTuna,
                IngredientName = "Pasta",
                Quantity = 2m,
                Unit = "packs",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a2020202-0202-0202-0202-020202020202"),
                RecipeId = RTuna,
                IngredientName = "Canned Tuna",
                Quantity = 2m,
                Unit = "cans",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a2020202-0202-0202-0202-020202020203"),
                RecipeId = RTuna,
                IngredientName = "Cheddar Cheese",
                Quantity = 150m,
                Unit = "g",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a2020202-0202-0202-0202-020202020204"),
                RecipeId = RTuna,
                IngredientName = "Tomatoes",
                Quantity = 4m,
                Unit = "units",
                IsOptional = false,
            },
            // Spinach & Egg Scramble
            new RecipeIngredient
            {
                Id = Guid.Parse("a3030303-0303-0303-0303-030303030301"),
                RecipeId = REggs,
                IngredientName = "Eggs",
                Quantity = 4m,
                Unit = "units",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a3030303-0303-0303-0303-030303030302"),
                RecipeId = REggs,
                IngredientName = "Spinach",
                Quantity = 1m,
                Unit = "bag",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a3030303-0303-0303-0303-030303030303"),
                RecipeId = REggs,
                IngredientName = "Butter",
                Quantity = 30m,
                Unit = "g",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a3030303-0303-0303-0303-030303030304"),
                RecipeId = REggs,
                IngredientName = "Onion",
                Quantity = 1m,
                Unit = "units",
                IsOptional = true,
            },
            // Chicken Fried Rice
            new RecipeIngredient
            {
                Id = Guid.Parse("a4040404-0404-0404-0404-040404040401"),
                RecipeId = RRice,
                IngredientName = "Rice",
                Quantity = 1m,
                Unit = "kg",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a4040404-0404-0404-0404-040404040402"),
                RecipeId = RRice,
                IngredientName = "Chicken Breast",
                Quantity = 2m,
                Unit = "units",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a4040404-0404-0404-0404-040404040403"),
                RecipeId = RRice,
                IngredientName = "Eggs",
                Quantity = 3m,
                Unit = "units",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a4040404-0404-0404-0404-040404040404"),
                RecipeId = RRice,
                IngredientName = "Onion",
                Quantity = 1m,
                Unit = "units",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a4040404-0404-0404-0404-040404040405"),
                RecipeId = RRice,
                IngredientName = "Garlic",
                Quantity = 2m,
                Unit = "cloves",
                IsOptional = false,
            },
            // Simple Tomato Pasta
            new RecipeIngredient
            {
                Id = Guid.Parse("a5050505-0505-0505-0505-050505050501"),
                RecipeId = RTomato,
                IngredientName = "Pasta",
                Quantity = 2m,
                Unit = "packs",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a5050505-0505-0505-0505-050505050502"),
                RecipeId = RTomato,
                IngredientName = "Tomatoes",
                Quantity = 6m,
                Unit = "units",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a5050505-0505-0505-0505-050505050503"),
                RecipeId = RTomato,
                IngredientName = "Garlic",
                Quantity = 4m,
                Unit = "cloves",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a5050505-0505-0505-0505-050505050504"),
                RecipeId = RTomato,
                IngredientName = "Olive Oil",
                Quantity = 3m,
                Unit = "tbsp",
                IsOptional = false,
            },
            // Classic Tuna Sandwich
            new RecipeIngredient
            {
                Id = Guid.Parse("a6060606-0606-0606-0606-060606060601"),
                RecipeId = RTunaSand,
                IngredientName = "Canned Tuna",
                Quantity = 2m,
                Unit = "cans",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a6060606-0606-0606-0606-060606060602"),
                RecipeId = RTunaSand,
                IngredientName = "Eggs",
                Quantity = 2m,
                Unit = "units",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a6060606-0606-0606-0606-060606060603"),
                RecipeId = RTunaSand,
                IngredientName = "Cheddar Cheese",
                Quantity = 50m,
                Unit = "g",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a6060606-0606-0606-0606-060606060604"),
                RecipeId = RTunaSand,
                IngredientName = "Onion",
                Quantity = 1m,
                Unit = "units",
                IsOptional = true,
            },
            // Garlic Butter Chicken
            new RecipeIngredient
            {
                Id = Guid.Parse("a7070707-0707-0707-0707-070707070701"),
                RecipeId = RChicken,
                IngredientName = "Chicken Breast",
                Quantity = 2m,
                Unit = "units",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a7070707-0707-0707-0707-070707070702"),
                RecipeId = RChicken,
                IngredientName = "Butter",
                Quantity = 60m,
                Unit = "g",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a7070707-0707-0707-0707-070707070703"),
                RecipeId = RChicken,
                IngredientName = "Garlic",
                Quantity = 3m,
                Unit = "cloves",
                IsOptional = false,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("a7070707-0707-0707-0707-070707070704"),
                RecipeId = RChicken,
                IngredientName = "Rice",
                Quantity = 1m,
                Unit = "kg",
                IsOptional = false,
            }
        );
    }
}
