using CookHomie.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookHomie.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public static readonly Guid RMac = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid RTuna = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid REggs = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    public static readonly Guid RRice = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    public static readonly Guid RTomato = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    public static readonly Guid RTunaSand = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    public static readonly Guid RChicken = Guid.Parse("77777777-7777-7777-7777-777777777777");

    private static readonly DateTime SeededAt = new(2026, 4, 26, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Instructions).IsRequired();
        builder.Property(e => e.Tags);
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasData(
            new Recipe
            {
                Id = RMac,
                Name = "Classic Mac & Cheese",
                Instructions =
                    "Boil pasta until al dente. In a saucepan, melt butter over medium heat. Stir in flour and cook 1 minute. Gradually whisk in milk and cook until thickened. Remove from heat, stir in cheddar until melted. Combine sauce with pasta, season with salt and pepper. Serve hot.",
                PrepMinutes = 10,
                CookMinutes = 20,
                Tags = new[] { "comfort", "quick", "vegetarian" },
                Source = null,
                CreatedAt = SeededAt,
            },
            new Recipe
            {
                Id = RTuna,
                Name = "Tuna Pasta Bake",
                Instructions =
                    "Preheat oven to 200\u00b0C. Cook pasta until al dente. Drain tuna. In a large bowl, mix pasta, tuna, tomatoes, and half the cheese. Transfer to a baking dish, top with remaining cheese. Bake 20 minutes until golden and bubbly.",
                PrepMinutes = 15,
                CookMinutes = 20,
                Tags = new[] { "baked", "protein", "family-friendly" },
                Source = null,
                CreatedAt = SeededAt,
            },
            new Recipe
            {
                Id = REggs,
                Name = "Spinach & Egg Scramble",
                Instructions =
                    "Beat eggs in a bowl with a splash of milk, salt, and pepper. Melt butter in a non-stick pan over medium-low heat. Add eggs and gently stir with a spatula. When nearly set, fold in spinach and cook 1 more minute. Serve immediately.",
                PrepMinutes = 5,
                CookMinutes = 8,
                Tags = new[] { "breakfast", "quick", "vegetarian" },
                Source = null,
                CreatedAt = SeededAt,
            },
            new Recipe
            {
                Id = RRice,
                Name = "Chicken Fried Rice",
                Instructions =
                    "Cook rice and let it cool (day-old rice works best). Scramble eggs and set aside. Heat oil in a wok over high heat. Stir-fry onion and garlic 1 minute. Add chicken and cook through. Add rice, soy sauce, and eggs. Toss everything together over high heat until well combined.",
                PrepMinutes = 15,
                CookMinutes = 15,
                Tags = new[] { "asian", "wok", "meal-prep" },
                Source = null,
                CreatedAt = SeededAt,
            },
            new Recipe
            {
                Id = RTomato,
                Name = "Simple Tomato Pasta",
                Instructions =
                    "Cook pasta in salted boiling water. Meanwhile, heat olive oil in a pan and saut\u00e9 garlic until fragrant. Add tomatoes and cook 10 minutes, crushing them lightly. Toss drained pasta with the sauce, adjust seasoning. Serve with fresh herbs if available.",
                PrepMinutes = 5,
                CookMinutes = 15,
                Tags = new[] { "quick", "vegan", "simple" },
                Source = null,
                CreatedAt = SeededAt,
            },
            new Recipe
            {
                Id = RTunaSand,
                Name = "Classic Tuna Sandwich",
                Instructions =
                    "Drain tuna and flake into a bowl. Hard boil eggs and chop. Mix tuna, eggs, cheddar, and onion. Add mayonnaise and stir until combined. Season to taste. Serve on bread with lettuce.",
                PrepMinutes = 15,
                CookMinutes = 10,
                Tags = new[] { "lunch", "sandwich", "protein" },
                Source = null,
                CreatedAt = SeededAt,
            },
            new Recipe
            {
                Id = RChicken,
                Name = "Garlic Butter Chicken with Rice",
                Instructions =
                    "Season chicken breast with salt and pepper. Melt butter in a skillet over medium-high heat. Cook chicken 5-7 minutes per side until golden and cooked through. In the last minute, add garlic and baste chicken. Rest 5 minutes, slice. Serve over steamed rice with pan juices drizzled on top.",
                PrepMinutes = 10,
                CookMinutes = 20,
                Tags = new[] { "protein", "garlic", "family-friendly" },
                Source = null,
                CreatedAt = SeededAt,
            }
        );
    }
}

