using System;
using CookHomie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookHomie.Infrastructure.Migrations;

/// <inheritdoc />
[DbContext(typeof(AppDbContext))]
[Migration("20260426193000_SeedInventoryBaselineData")]
public partial class SeedInventoryBaselineData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "InventoryItems",
            columns: new[] { "Id", "Category", "CreatedAt", "ExpiresAt", "IsOpened", "Location", "Name", "Notes", "Quantity", "Unit", "UpdatedAt" },
            values: new object[,]
            {
                { new Guid("d8109ce9-f967-4f32-a4a4-5031a0df1baf"), "Dairy", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "Milk", null, 1m, "liter", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("5a4b2996-ebf8-4b98-91fe-290ca2d9b2bd"), "Dairy", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 1, "Cheddar Cheese", null, 250m, "g", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("4f80dd83-a339-45bb-9f24-d76cc0f25818"), "Produce", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "Spinach", null, 1m, "bag", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("680aef2a-9969-4346-95d0-c6fc9f426445"), "Produce", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "Tomatoes", null, 6m, "units", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("fd769e8e-252a-4e14-b0ec-87a5f4baa253"), "Protein", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 2, "Chicken Breast", null, 2m, "units", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("933d0ce1-faad-4a4b-b349-1d9f60dbb0be"), "Protein", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 0, "Canned Tuna", null, 3m, "cans", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("f6f06cae-72f7-4bc2-bf16-58b7d24924fe"), "Grains", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 0, "Rice", null, 2m, "kg", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                { new Guid("f92ff908-beb0-4af0-90d9-5992a39cc0bc"), "Grains", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 0, "Pasta", null, 4m, "packs", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc) }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "InventoryItems", keyColumn: "Id", keyValue: new Guid("d8109ce9-f967-4f32-a4a4-5031a0df1baf"));
        migrationBuilder.DeleteData(table: "InventoryItems", keyColumn: "Id", keyValue: new Guid("5a4b2996-ebf8-4b98-91fe-290ca2d9b2bd"));
        migrationBuilder.DeleteData(table: "InventoryItems", keyColumn: "Id", keyValue: new Guid("4f80dd83-a339-45bb-9f24-d76cc0f25818"));
        migrationBuilder.DeleteData(table: "InventoryItems", keyColumn: "Id", keyValue: new Guid("680aef2a-9969-4346-95d0-c6fc9f426445"));
        migrationBuilder.DeleteData(table: "InventoryItems", keyColumn: "Id", keyValue: new Guid("fd769e8e-252a-4e14-b0ec-87a5f4baa253"));
        migrationBuilder.DeleteData(table: "InventoryItems", keyColumn: "Id", keyValue: new Guid("933d0ce1-faad-4a4b-b349-1d9f60dbb0be"));
        migrationBuilder.DeleteData(table: "InventoryItems", keyColumn: "Id", keyValue: new Guid("f6f06cae-72f7-4bc2-bf16-58b7d24924fe"));
        migrationBuilder.DeleteData(table: "InventoryItems", keyColumn: "Id", keyValue: new Guid("f92ff908-beb0-4af0-90d9-5992a39cc0bc"));
    }
}
