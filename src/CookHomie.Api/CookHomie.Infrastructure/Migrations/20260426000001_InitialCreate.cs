using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookHomie.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InventoryItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Category = table.Column<string>(type: "text", nullable: false),
                Location = table.Column<int>(type: "integer", nullable: false),
                Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                Unit = table.Column<string>(type: "text", nullable: false),
                ExpiresAt = table.Column<DateOnly>(type: "date", nullable: true),
                IsOpened = table.Column<bool>(type: "boolean", nullable: false),
                Notes = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InventoryItems", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Recipes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Instructions = table.Column<string>(type: "text", nullable: false),
                PrepMinutes = table.Column<int>(type: "integer", nullable: false),
                CookMinutes = table.Column<int>(type: "integer", nullable: false),
                Tags = table.Column<string[]>(type: "text[]", nullable: false),
                Source = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Recipes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ShoppingItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Quantity = table.Column<decimal>(type: "numeric", nullable: true),
                Unit = table.Column<string>(type: "text", nullable: true),
                Priority = table.Column<int>(type: "integer", nullable: false),
                IsBought = table.Column<bool>(type: "boolean", nullable: false),
                LinkedRecipeId = table.Column<Guid>(type: "uuid", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShoppingItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_ShoppingItems_Recipes_LinkedRecipeId",
                    column: x => x.LinkedRecipeId,
                    principalTable: "Recipes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "RecipeIngredients",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                IngredientName = table.Column<string>(type: "text", nullable: false),
                Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                Unit = table.Column<string>(type: "text", nullable: false),
                IsOptional = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RecipeIngredients", x => x.Id);
                table.ForeignKey(
                    name: "FK_RecipeIngredients_Recipes_RecipeId",
                    column: x => x.RecipeId,
                    principalTable: "Recipes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InventoryItems");

        migrationBuilder.DropTable(
            name: "RecipeIngredients");

        migrationBuilder.DropTable(
            name: "ShoppingItems");

        migrationBuilder.DropTable(
            name: "Recipes");
    }
}