using CookHomie.Domain.Interfaces;
using CookHomie.Infrastructure.Persistence;
using CookHomie.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using CookHomie.Application.UseCases.Inventory;
using CookHomie.Application.UseCases.Recipes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<AddInventoryItemUseCase>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<CreateRecipeUseCase>();
builder.Services.AddScoped<UpdateRecipeUseCase>();
builder.Services.AddScoped<GetMissingIngredientsUseCase>();

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
