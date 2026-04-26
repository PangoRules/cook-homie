# Task 8: EF Core Infrastructure + Initial Migration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add production-ready EF Core infrastructure for the full v1 data model and generate a single `InitialCreate` migration (tables + PK/FK/nullability only).

**Architecture:** Expand domain entities to match `docs/02-DataModel.md`, wire EF Core in `CookHomie.Infrastructure`, keep application behavior unchanged with persistence now routed through real repository + DbContext. Validate with infrastructure integration tests before migration and app startup wiring.

**Tech Stack:** .NET 10, EF Core 10, Npgsql EF provider, xUnit, Testcontainers PostgreSQL (optional but recommended for tests).

---

## File Map (Create/Modify Responsibilities)

- `src/CookHomie.Api/CookHomie.Domain/` — expand entities to v1 shape.
- `src/CookHomie.Api/CookHomie.Infrastructure/` — add AppDbContext, repositories, migrations.
- `src/CookHomie.Api/CookHomie.WebApi/` — wire DI, connection string.
- `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/` — new test project for persistence validation.

---

### Task 1: Create Branch + Scaffold Infrastructure Test Project

**Files:**
- Modify: `src/CookHomie.Api/CookHomie.sln`
- Create: `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/CookHomie.Infrastructure.Tests.csproj`
- Create: `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/GlobalUsings.cs`

- [ ] **Step 1: Create branch**

```bash
git checkout -b feat/add-efcore-infra-initial-migration
```

- [ ] **Step 2: Create test project directory**

```bash
mkdir -p src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests
```

- [ ] **Step 3: Create test project file**

Create `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/CookHomie.Infrastructure.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.4" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\CookHomie.Domain\CookHomie.Domain.csproj" />
    <ProjectReference Include="..\..\CookHomie.Infrastructure\CookHomie.Infrastructure.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 4: Create GlobalUsings.cs**

Create `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/GlobalUsings.cs`:

```csharp
global using CookHomie.Domain.Entities;
global using CookHomie.Domain.Enums;
global using CookHomie.Domain.Interfaces;
global using CookHomie.Infrastructure.Persistence;
global using CookHomie.Infrastructure.Repositories;
global using Xunit;
```

- [ ] **Step 5: Add test project to solution**

```bash
cd src/CookHomie.Api
dotnet sln CookHomie.sln add tests/CookHomie.Infrastructure.Tests/CookHomie.Infrastructure.Tests.csproj
cd ../../..
```

- [ ] **Step 6: Verify solution lists test project**

```bash
dotnet sln src/CookHomie.Api/CookHomie.sln list | grep Infrastructure.Tests
```

Expected: line containing `CookHomie.Infrastructure.Tests`

---

### Task 2: Expand Domain Model to Full v1 Shape

**Files:**
- Modify: `src/CookHomie.Api/CookHomie.Domain/Entities/InventoryItem.cs`
- Create: `src/CookHomie.Api/CookHomie.Domain/Entities/Recipe.cs`
- Create: `src/CookHomie.Api/CookHomie.Domain/Entities/RecipeIngredient.cs`
- Create: `src/CookHomie.Api/CookHomie.Domain/Entities/ShoppingItem.cs`
- Create: `src/CookHomie.Api/CookHomie.Domain/Enums/Priority.cs`
- Modify: `src/CookHomie.Api/CookHomie.Domain/Enums/Location.cs`

- [ ] **Step 1: Update InventoryItem.cs to full shape**

Replace entire file at `src/CookHomie.Api/CookHomie.Domain/Entities/InventoryItem.cs`:

```csharp
using CookHomie.Domain.Enums;

namespace CookHomie.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public Location Location { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public DateOnly? ExpiresAt { get; set; }

    public bool IsOpened { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
```

- [ ] **Step 2: Update Location.cs enum**

Modify `src/CookHomie.Api/CookHomie.Domain/Enums/Location.cs`:

```csharp
namespace CookHomie.Domain.Enums;

public enum Location
{
    Pantry,
    Fridge,
    Freezer,
    Spices
}
```

- [ ] **Step 3: Create Priority.cs enum**

Create `src/CookHomie.Api/CookHomie.Domain/Enums/Priority.cs`:

```csharp
namespace CookHomie.Domain.Enums;

public enum Priority
{
    Low,
    Normal,
    High
}
```

- [ ] **Step 4: Create Recipe.cs entity**

Create `src/CookHomie.Api/CookHomie.Domain/Entities/Recipe.cs`:

```csharp
namespace CookHomie.Domain.Entities;

public class Recipe
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public int PrepMinutes { get; set; }

    public int CookMinutes { get; set; }

    public string[] Tags { get; set; } = [];

    public string? Source { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation property
    public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
}
```

- [ ] **Step 5: Create RecipeIngredient.cs entity**

Create `src/CookHomie.Api/CookHomie.Domain/Entities/RecipeIngredient.cs`:

```csharp
namespace CookHomie.Domain.Entities;

public class RecipeIngredient
{
    public Guid Id { get; set; }

    public Guid RecipeId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public bool IsOptional { get; set; }

    // Navigation property
    public Recipe? Recipe { get; set; }
}
```

- [ ] **Step 6: Create ShoppingItem.cs entity**

Create `src/CookHomie.Api/CookHomie.Domain/Entities/ShoppingItem.cs`:

```csharp
using CookHomie.Domain.Enums;

namespace CookHomie.Domain.Entities;

public class ShoppingItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public Priority Priority { get; set; }

    public bool IsBought { get; set; }

    public Guid? LinkedRecipeId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation property
    public Recipe? LinkedRecipe { get; set; }
}
```

- [ ] **Step 7: Verify domain builds**

```bash
dotnet build src/CookHomie.Api/CookHomie.Domain/CookHomie.Domain.csproj
```

Expected: build succeeds

---

### Task 3: Add EF Core Dependencies + AppDbContext

**Files:**
- Modify: `src/CookHomie.Api/CookHomie.Infrastructure/CookHomie.Infrastructure.csproj`
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/Persistence/AppDbContext.cs`

- [ ] **Step 1: Add EF Core packages to Infrastructure.csproj**

Update entire `src/CookHomie.Api/CookHomie.Infrastructure/CookHomie.Infrastructure.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\CookHomie.Domain\CookHomie.Domain.csproj" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.4" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.4" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.2" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Create Persistence directory**

```bash
mkdir -p src/CookHomie.Api/CookHomie.Infrastructure/Persistence
```

- [ ] **Step 3: Create AppDbContext.cs**

Create `src/CookHomie.Api/CookHomie.Infrastructure/Persistence/AppDbContext.cs`:

```csharp
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
        });

        // Recipe
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Instructions).IsRequired();
            entity.Property(e => e.Tags).HasConversion(
                v => v,
                v => v
            );
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
```

- [ ] **Step 4: Verify Infrastructure builds**

```bash
dotnet build src/CookHomie.Api/CookHomie.Infrastructure/CookHomie.Infrastructure.csproj
```

Expected: build succeeds

---

### Task 4: Implement InventoryRepository

**Files:**
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/Repositories/InventoryRepository.cs`

- [ ] **Step 1: Create Repositories directory**

```bash
mkdir -p src/CookHomie.Api/CookHomie.Infrastructure/Repositories
```

- [ ] **Step 2: Create InventoryRepository.cs**

Create `src/CookHomie.Api/CookHomie.Infrastructure/Repositories/InventoryRepository.cs`:

```csharp
using CookHomie.Domain.Entities;
using CookHomie.Domain.Interfaces;
using CookHomie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _context;

    public InventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem> AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }
}
```

- [ ] **Step 3: Verify Infrastructure builds**

```bash
dotnet build src/CookHomie.Api/CookHomie.Infrastructure/CookHomie.Infrastructure.csproj
```

Expected: build succeeds

---

### Task 5: Write Failing Integration Tests, Then Pass Them

**Files:**
- Create: `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/Persistence/TestDbFactory.cs`
- Create: `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/InventoryRepositoryTests.cs`

- [ ] **Step 1: Create Persistence test directory**

```bash
mkdir -p src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/Persistence
```

- [ ] **Step 2: Create TestDbFactory.cs**

Create `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/Persistence/TestDbFactory.cs`:

```csharp
using CookHomie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CookHomie.Infrastructure.Tests.Persistence;

public static class TestDbFactory
{
    public static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
```

- [ ] **Step 3: Create InventoryRepositoryTests.cs (failing test first)**

Create `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/InventoryRepositoryTests.cs`:

```csharp
namespace CookHomie.Infrastructure.Tests;

public class InventoryRepositoryTests
{
    [Fact]
    public async Task AddAsync_PersistsInventoryItem()
    {
        // Arrange
        await using var db = TestDbFactory.CreateContext();
        var repo = new InventoryRepository(db);
        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = "Eggs",
            Category = "Dairy",
            Location = Location.Fridge,
            Quantity = 12m,
            Unit = "units",
            IsOpened = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        await repo.AddAsync(item);

        // Assert
        var stored = await db.InventoryItems.FirstOrDefaultAsync(i => i.Id == item.Id);
        Assert.NotNull(stored);
        Assert.Equal("Eggs", stored.Name);
        Assert.Equal(Location.Fridge, stored.Location);
    }

    [Fact]
    public async Task AddAsync_WithRecipeForeignKey_PreservesForeignKeyIntegrity()
    {
        // Arrange
        await using var db = TestDbFactory.CreateContext();
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Name = "Scrambled Eggs",
            Instructions = "Beat and fry",
            PrepMinutes = 5,
            CookMinutes = 10,
            Tags = ["breakfast", "quick"],
            CreatedAt = DateTime.UtcNow
        };
        
        var ingredient = new RecipeIngredient
        {
            Id = Guid.NewGuid(),
            RecipeId = recipe.Id,
            IngredientName = "Eggs",
            Quantity = 3m,
            Unit = "units",
            IsOptional = false
        };

        // Act
        db.Recipes.Add(recipe);
        db.RecipeIngredients.Add(ingredient);
        await db.SaveChangesAsync();

        // Assert
        var stored = await db.RecipeIngredients.FirstOrDefaultAsync(ri => ri.Id == ingredient.Id);
        Assert.NotNull(stored);
        Assert.Equal(recipe.Id, stored.RecipeId);
        var linkedRecipe = await db.Recipes.FirstOrDefaultAsync(r => r.Id == stored.RecipeId);
        Assert.NotNull(linkedRecipe);
    }
}
```

- [ ] **Step 4: Run tests (expect PASS now that repository + context exist)**

```bash
dotnet test src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/CookHomie.Infrastructure.Tests.csproj -v
```

Expected: both tests PASS

---

### Task 6: Wire WebApi DI + Connection String

**Files:**
- Modify: `src/CookHomie.Api/CookHomie.WebApi/Program.cs`
- Modify: `src/CookHomie.Api/CookHomie.WebApi/appsettings.json`
- Modify: `src/CookHomie.Api/CookHomie.WebApi/appsettings.Development.json`

- [ ] **Step 1: Update appsettings.json with connection string placeholder**

Replace entire `src/CookHomie.Api/CookHomie.WebApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=cookhomie;Username=cookhomie;Password=cookhomie_dev_password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

- [ ] **Step 2: Update appsettings.Development.json**

Replace entire `src/CookHomie.Api/CookHomie.WebApi/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

- [ ] **Step 3: Update Program.cs with DI wiring**

Replace entire `src/CookHomie.Api/CookHomie.WebApi/Program.cs`:

```csharp
using CookHomie.Application.UseCases.Inventory;
using CookHomie.Domain.Interfaces;
using CookHomie.Infrastructure.Persistence;
using CookHomie.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not configured in appsettings.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Repositories
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

// Use cases
builder.Services.AddScoped<AddInventoryItemUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok());

app.Run();
```

- [ ] **Step 4: Verify WebApi builds**

```bash
dotnet build src/CookHomie.Api/CookHomie.WebApi/CookHomie.WebApi.csproj
```

Expected: build succeeds

---

### Task 7: Generate InitialCreate Migration

**Files:**
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/Migrations/*`

- [ ] **Step 1: Ensure full solution builds**

```bash
dotnet build src/CookHomie.Api/CookHomie.sln
```

Expected: build succeeds with no errors

- [ ] **Step 2: Generate migration**

```bash
cd src/CookHomie.Api && dotnet ef migrations add InitialCreate --project CookHomie.Infrastructure --startup-project CookHomie.WebApi && cd ../../..
```

Expected: migration files created under `src/CookHomie.Api/CookHomie.Infrastructure/Migrations/`

- [ ] **Step 3: Review migration for scope**

```bash
ls -la src/CookHomie.Api/CookHomie.Infrastructure/Migrations/
```

Expected: `<timestamp>_InitialCreate.cs` and `AppDbContextModelSnapshot.cs` exist

- [ ] **Step 4: Inspect migration content briefly**

Open `src/CookHomie.Api/CookHomie.Infrastructure/Migrations/<timestamp>_InitialCreate.cs` and verify:
- Creates `InventoryItems`, `Recipes`, `RecipeIngredients`, `ShoppingItems` tables
- Defines primary keys (all Guid)
- Defines foreign keys (`RecipeIngredients.RecipeId -> Recipes.Id`, `ShoppingItems.LinkedRecipeId -> Recipes.Id`)
- No extra indexes or constraints beyond those

---

### Task 8: Full Verification + Commit

**Files:**
- (All previous tasks)

- [ ] **Step 1: Run full solution build**

```bash
dotnet build src/CookHomie.Api/CookHomie.sln
```

Expected: build succeeds

- [ ] **Step 2: Run all Application tests**

```bash
dotnet test src/CookHomie.Api/tests/CookHomie.Application.Tests/CookHomie.Application.Tests.csproj -v
```

Expected: all Application tests pass (existing use case tests should still work)

- [ ] **Step 3: Run all Infrastructure tests**

```bash
dotnet test src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/CookHomie.Infrastructure.Tests.csproj -v
```

Expected: both persistence + FK tests pass

- [ ] **Step 4: Run full solution tests**

```bash
dotnet test src/CookHomie.Api/CookHomie.sln -v
```

Expected: all tests pass

- [ ] **Step 5: Commit all changes**

```bash
cd src/CookHomie.Api && git add . && cd ../../.. && git add .
git commit -m "feat: add ef core infrastructure and initial migration

- Expand domain entities to full v1 model (InventoryItem, Recipe, RecipeIngredient, ShoppingItem)
- Implement AppDbContext with all entity mappings (table names, PK/FK, required/nullable)
- Create InventoryRepository implementing IInventoryRepository
- Add CookHomie.Infrastructure.Tests project with persistence + FK integrity tests
- Wire EF Core DI in Program.cs with connection string validation
- Generate InitialCreate migration covering tables and keys only
- All tests passing, solution builds clean"
```

Expected: commit succeeds

- [ ] **Step 6: Verify branch status**

```bash
git log --oneline -n 2
git status
```

Expected: branch is `feat/add-efcore-infra-initial-migration`, working tree clean, commit visible in log
