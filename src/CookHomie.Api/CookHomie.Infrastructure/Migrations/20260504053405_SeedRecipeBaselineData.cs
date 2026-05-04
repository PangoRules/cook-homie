using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CookHomie.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRecipeBaselineData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "InventoryItems",
                columns: new[] { "Id", "Category", "CreatedAt", "ExpiresAt", "IsOpened", "Location", "Name", "Notes", "Quantity", "Unit", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Dairy", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "Eggs", null, 12m, "units", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Dairy", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "Butter", null, 200m, "g", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Produce", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 0, "Garlic", null, 1m, "head", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Pantry", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 0, "Olive Oil", null, 500m, "ml", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "CookMinutes", "CreatedAt", "Instructions", "Name", "PrepMinutes", "Source", "Tags" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), 20, new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Season chicken breast with salt and pepper. Melt butter in a skillet over medium-high heat. Cook chicken 5-7 minutes per side until golden and cooked through. In the last minute, add garlic and baste chicken. Rest 5 minutes, slice. Serve over steamed rice with pan juices drizzled on top.", "Garlic Butter Chicken with Rice", 10, null, new[] { "protein", "garlic", "family-friendly" } },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 20, new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Boil pasta until al dente. In a saucepan, melt butter over medium heat. Stir in flour and cook 1 minute. Gradually whisk in milk and cook until thickened. Remove from heat, stir in cheddar until melted. Combine sauce with pasta, season with salt and pepper. Serve hot.", "Classic Mac & Cheese", 10, null, new[] { "comfort", "quick", "vegetarian" } },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 20, new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Preheat oven to 200°C. Cook pasta until al dente. Drain tuna. In a large bowl, mix pasta, tuna, tomatoes, and half the cheese. Transfer to a baking dish, top with remaining cheese. Bake 20 minutes until golden and bubbly.", "Tuna Pasta Bake", 15, null, new[] { "baked", "protein", "family-friendly" } },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 8, new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Beat eggs in a bowl with a splash of milk, salt, and pepper. Melt butter in a non-stick pan over medium-low heat. Add eggs and gently stir with a spatula. When nearly set, fold in spinach and cook 1 more minute. Serve immediately.", "Spinach & Egg Scramble", 5, null, new[] { "breakfast", "quick", "vegetarian" } },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), 15, new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Cook rice and let it cool (day-old rice works best). Scramble eggs and set aside. Heat oil in a wok over high heat. Stir-fry onion and garlic 1 minute. Add chicken and cook through. Add rice, soy sauce, and eggs. Toss everything together over high heat until well combined.", "Chicken Fried Rice", 15, null, new[] { "asian", "wok", "meal-prep" } },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 15, new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Cook pasta in salted boiling water. Meanwhile, heat olive oil in a pan and sauté garlic until fragrant. Add tomatoes and cook 10 minutes, crushing them lightly. Toss drained pasta with the sauce, adjust seasoning. Serve with fresh herbs if available.", "Simple Tomato Pasta", 5, null, new[] { "quick", "vegan", "simple" } },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), 10, new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Drain tuna and flake into a bowl. Hard boil eggs and chop. Mix tuna, eggs, cheddar, and onion. Add mayonnaise and stir until combined. Season to taste. Serve on bread with lettuce.", "Classic Tuna Sandwich", 15, null, new[] { "lunch", "sandwich", "protein" } }
                });

            migrationBuilder.InsertData(
                table: "RecipeIngredients",
                columns: new[] { "Id", "IngredientName", "IsOptional", "Quantity", "RecipeId", "Unit" },
                values: new object[,]
                {
                    { new Guid("a1010101-0101-0101-0101-010101010101"), "Pasta", false, 2m, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "packs" },
                    { new Guid("a1010101-0101-0101-0101-010101010102"), "Cheddar Cheese", false, 200m, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "g" },
                    { new Guid("a1010101-0101-0101-0101-010101010103"), "Butter", false, 50m, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "g" },
                    { new Guid("a1010101-0101-0101-0101-010101010104"), "Milk", false, 1m, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "liter" },
                    { new Guid("a2020202-0202-0202-0202-020202020201"), "Pasta", false, 2m, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "packs" },
                    { new Guid("a2020202-0202-0202-0202-020202020202"), "Canned Tuna", false, 2m, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "cans" },
                    { new Guid("a2020202-0202-0202-0202-020202020203"), "Cheddar Cheese", false, 150m, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "g" },
                    { new Guid("a2020202-0202-0202-0202-020202020204"), "Tomatoes", false, 4m, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "units" },
                    { new Guid("a3030303-0303-0303-0303-030303030301"), "Eggs", false, 4m, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "units" },
                    { new Guid("a3030303-0303-0303-0303-030303030302"), "Spinach", false, 1m, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "bag" },
                    { new Guid("a3030303-0303-0303-0303-030303030303"), "Butter", false, 30m, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "g" },
                    { new Guid("a3030303-0303-0303-0303-030303030304"), "Onion", true, 1m, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "units" },
                    { new Guid("a4040404-0404-0404-0404-040404040401"), "Rice", false, 1m, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "kg" },
                    { new Guid("a4040404-0404-0404-0404-040404040402"), "Chicken Breast", false, 2m, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "units" },
                    { new Guid("a4040404-0404-0404-0404-040404040403"), "Eggs", false, 3m, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "units" },
                    { new Guid("a4040404-0404-0404-0404-040404040404"), "Onion", false, 1m, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "units" },
                    { new Guid("a4040404-0404-0404-0404-040404040405"), "Garlic", false, 2m, new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "cloves" },
                    { new Guid("a5050505-0505-0505-0505-050505050501"), "Pasta", false, 2m, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "packs" },
                    { new Guid("a5050505-0505-0505-0505-050505050502"), "Tomatoes", false, 6m, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "units" },
                    { new Guid("a5050505-0505-0505-0505-050505050503"), "Garlic", false, 4m, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "cloves" },
                    { new Guid("a5050505-0505-0505-0505-050505050504"), "Olive Oil", false, 3m, new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "tbsp" },
                    { new Guid("a6060606-0606-0606-0606-060606060601"), "Canned Tuna", false, 2m, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "cans" },
                    { new Guid("a6060606-0606-0606-0606-060606060602"), "Eggs", false, 2m, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "units" },
                    { new Guid("a6060606-0606-0606-0606-060606060603"), "Cheddar Cheese", false, 50m, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "g" },
                    { new Guid("a6060606-0606-0606-0606-060606060604"), "Onion", true, 1m, new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "units" },
                    { new Guid("a7070707-0707-0707-0707-070707070701"), "Chicken Breast", false, 2m, new Guid("77777777-7777-7777-7777-777777777777"), "units" },
                    { new Guid("a7070707-0707-0707-0707-070707070702"), "Butter", false, 60m, new Guid("77777777-7777-7777-7777-777777777777"), "g" },
                    { new Guid("a7070707-0707-0707-0707-070707070703"), "Garlic", false, 3m, new Guid("77777777-7777-7777-7777-777777777777"), "cloves" },
                    { new Guid("a7070707-0707-0707-0707-070707070704"), "Rice", false, 1m, new Guid("77777777-7777-7777-7777-777777777777"), "kg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a1010101-0101-0101-0101-010101010101"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a1010101-0101-0101-0101-010101010102"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a1010101-0101-0101-0101-010101010103"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a1010101-0101-0101-0101-010101010104"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a2020202-0202-0202-0202-020202020201"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a2020202-0202-0202-0202-020202020202"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a2020202-0202-0202-0202-020202020203"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a2020202-0202-0202-0202-020202020204"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a3030303-0303-0303-0303-030303030301"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a3030303-0303-0303-0303-030303030302"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a3030303-0303-0303-0303-030303030303"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a3030303-0303-0303-0303-030303030304"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a4040404-0404-0404-0404-040404040401"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a4040404-0404-0404-0404-040404040402"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a4040404-0404-0404-0404-040404040403"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a4040404-0404-0404-0404-040404040404"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a4040404-0404-0404-0404-040404040405"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a5050505-0505-0505-0505-050505050501"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a5050505-0505-0505-0505-050505050502"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a5050505-0505-0505-0505-050505050503"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a5050505-0505-0505-0505-050505050504"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a6060606-0606-0606-0606-060606060601"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a6060606-0606-0606-0606-060606060602"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a6060606-0606-0606-0606-060606060603"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a6060606-0606-0606-0606-060606060604"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a7070707-0707-0707-0707-070707070701"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a7070707-0707-0707-0707-070707070702"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a7070707-0707-0707-0707-070707070703"));

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: new Guid("a7070707-0707-0707-0707-070707070704"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"));
        }
    }
}
