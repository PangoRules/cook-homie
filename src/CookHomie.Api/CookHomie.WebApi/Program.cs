using CookHomie.Domain.Interfaces;
using CookHomie.Infrastructure.Persistence;
using CookHomie.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using CookHomie.Domain.Entities;
using CookHomie.Domain.Enums;
using System.Diagnostics;
using CookHomie.Application.UseCases.Inventory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<AddInventoryItemUseCase>();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
    else
    {
        dbContext.Database.EnsureCreated();
    }

    if (!dbContext.InventoryItems.Any())
    {
        var seededAt = DateTime.UtcNow;
        dbContext.InventoryItems.AddRange(
            new InventoryItem { Id = Guid.Parse("d8109ce9-f967-4f32-a4a4-5031a0df1baf"), Name = "Milk", Category = "Dairy", Location = Location.Fridge, Quantity = 1m, Unit = "liter", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("5a4b2996-ebf8-4b98-91fe-290ca2d9b2bd"), Name = "Cheddar Cheese", Category = "Dairy", Location = Location.Fridge, Quantity = 250m, Unit = "g", IsOpened = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("4f80dd83-a339-45bb-9f24-d76cc0f25818"), Name = "Spinach", Category = "Produce", Location = Location.Fridge, Quantity = 1m, Unit = "bag", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("680aef2a-9969-4346-95d0-c6fc9f426445"), Name = "Tomatoes", Category = "Produce", Location = Location.Fridge, Quantity = 6m, Unit = "units", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("fd769e8e-252a-4e14-b0ec-87a5f4baa253"), Name = "Chicken Breast", Category = "Protein", Location = Location.Freezer, Quantity = 2m, Unit = "units", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("933d0ce1-faad-4a4b-b349-1d9f60dbb0be"), Name = "Canned Tuna", Category = "Protein", Location = Location.Pantry, Quantity = 3m, Unit = "cans", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("f6f06cae-72f7-4bc2-bf16-58b7d24924fe"), Name = "Rice", Category = "Grains", Location = Location.Pantry, Quantity = 2m, Unit = "kg", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("f92ff908-beb0-4af0-90d9-5992a39cc0bc"), Name = "Pasta", Category = "Grains", Location = Location.Pantry, Quantity = 4m, Unit = "packs", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt });
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/swagger/{documentName}/swagger.json");

    const string swaggerUiHtml = """
<!doctype html>
<html>
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>CookHomie API Docs</title>
    <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
  </head>
  <body>
    <div id="swagger-ui"></div>
    <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
    <script>
      window.ui = SwaggerUIBundle({
        url: '/swagger/v1/swagger.json',
        dom_id: '#swagger-ui'
      });
    </script>
  </body>
</html>
""";

    app.MapGet("/swagger", () => Results.Content(swaggerUiHtml, "text/html")).ExcludeFromDescription();
    app.MapGet("/swagger/index.html", () => Results.Content(swaggerUiHtml, "text/html")).ExcludeFromDescription();
}

app.MapControllers();
app.MapGet("/health", async (AppDbContext dbContext, CancellationToken cancellationToken) =>
{
    var checks = new Dictionary<string, string>
    {
        ["api"] = "ok"
    };

    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        if (!canConnect)
        {
            checks["database"] = "unreachable";
            return Results.Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Service Unhealthy",
                detail: "Database connectivity check failed.",
                extensions: new Dictionary<string, object?>
                {
                    ["status"] = "unhealthy",
                    ["checks"] = checks,
                    ["traceId"] = Activity.Current?.Id ?? string.Empty
                });
        }
    }
    catch (Exception ex)
    {
        checks["database"] = "error";
        return Results.Problem(
            statusCode: StatusCodes.Status503ServiceUnavailable,
            title: "Service Unhealthy",
            detail: "Database connectivity check threw an exception.",
            extensions: new Dictionary<string, object?>
            {
                ["status"] = "unhealthy",
                ["checks"] = checks,
                ["error"] = ex.Message,
                ["traceId"] = Activity.Current?.Id ?? string.Empty
            });
    }

    checks["database"] = "ok";
    return Results.Ok(new
    {
        status = "ok",
        checks,
        traceId = Activity.Current?.Id ?? string.Empty
    });
});

app.Run();

public partial class Program;
