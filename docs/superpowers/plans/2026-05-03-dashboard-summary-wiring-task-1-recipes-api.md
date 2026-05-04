# Task 1: Implement C# RecipesController, repository/use cases, Nuxt recipe proxy migration, and tests
**Branch:** `task/m2-1-recipes-api`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-dashboard-summary-wiring-design.md`

# Recipes API Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Persist recipes through C# API and make Nuxt recipe routes thin proxies.

**Architecture:** Keep dependency direction inward: Domain owns repository abstraction, Application owns DTOs/use cases/validation, Infrastructure implements EF repository, WebApi owns controller/HTTP mapping. Nuxt routes proxy only; delete in-memory recipe source after proxy tests pass.

**Tech Stack:** ASP.NET Core 10 controllers, EF Core, xUnit integration tests, Nuxt 3 server routes, Vitest.

---

## Files

- Create: `src/CookHomie.Api/CookHomie.Domain/Interfaces/IRecipeRepository.cs`
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/Repositories/RecipeRepository.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/DTOs/RecipeDtos.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/UseCases/Recipes/CreateRecipeUseCase.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/UseCases/Recipes/UpdateRecipeUseCase.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/UseCases/Recipes/GetMissingIngredientsUseCase.cs`
- Create: `src/CookHomie.Api/CookHomie.WebApi/Controllers/RecipesController.cs`
- Modify: `src/CookHomie.Api/CookHomie.WebApi/Program.cs`
- Create: `src/CookHomie.Api/tests/CookHomie.WebApi.Tests/RecipesControllerTests.cs`
- Modify: `src/CookHomie.Web/server/api/recipes/index.get.ts`
- Modify: `src/CookHomie.Web/server/api/recipes/index.post.ts`
- Modify: `src/CookHomie.Web/server/api/recipes/[id].get.ts`
- Modify: `src/CookHomie.Web/server/api/recipes/[id].patch.ts`
- Modify: `src/CookHomie.Web/server/api/recipes/[id]/missing.get.ts`
- Delete: `src/CookHomie.Web/server/utils/recipeStore.ts`
- Replace: `src/CookHomie.Web/tests/recipe-store.spec.ts` with recipe proxy tests or delete and create `src/CookHomie.Web/tests/recipe-proxy.spec.ts`

## Parallelizable?

No. Backend contract must land before Nuxt proxy migration. After DTO shape is fixed, proxy tests and route edits can be done in same task branch.

### Task 1.1: Add failing backend integration tests

- [ ] Add `RecipesControllerTests.cs` with WebApplicationFactory setup copied from `InventoryControllerTests.cs`; in constructor remove existing `DbContextOptions<AppDbContext>`, add InMemory DB with unique name/root, call `EnsureCreated()`, clear `Recipes`, `RecipeIngredients`, `InventoryItems`, then seed one recipe and inventory item.
- [ ] Include tests:
  - `GetRecipes_ReturnsPersistedRecipesWithIngredients`
  - `GetRecipe_WithKnownId_ReturnsRecipe`
  - `GetRecipe_WithMissingId_ReturnsNotFoundProblemDetails`
  - `PostRecipe_PersistsRecipeAndIngredients`
  - `PostRecipe_WithBlankName_ReturnsBadRequestProblemDetails`
  - `PatchRecipe_ReplacesIngredientsWhenSupplied`
  - `GetMissingIngredients_UsesRequiredExactNormalizedInventoryNames`
- [ ] Use request/response helper classes in test file matching frontend types: `id`, `name`, `instructions`, `prepMinutes`, `cookMinutes`, `tags`, `source`, `ingredients`.
- [ ] Run: `dotnet test CookHomie.sln --filter RecipesControllerTests`
- [ ] Expected: build fails because `RecipesController` does not exist.

### Task 1.2: Add Domain repository abstraction

- [ ] Create `IRecipeRepository`:

```csharp
using CookHomie.Domain.Entities;

namespace CookHomie.Domain.Interfaces;

public interface IRecipeRepository
{
    Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Recipe> AddAsync(Recipe recipe, CancellationToken cancellationToken = default);
    Task<Recipe?> UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default);
}
```

### Task 1.3: Add Application DTOs and use cases

- [ ] Create `RecipeDtos.cs` containing `RecipeDto`, `RecipeIngredientDto`, `UpsertRecipeRequest`, `UpsertRecipeIngredientRequest`.
- [ ] Use nullable `Guid? Id` on ingredient upsert requests so frontend can send edited existing ingredients or new rows.
- [ ] Add mapper extension/internal static mapper in same file or use case namespace: map entity → DTO with ingredient list ordered by `IngredientName`.
- [ ] Create `CreateRecipeUseCase`. Validate: nonblank `Name`, nonblank `Instructions`, `PrepMinutes >= 0`, `CookMinutes >= 0`, at least one ingredient, each ingredient has nonblank `IngredientName`, `Unit`, and `Quantity > 0`. Trim strings, remove blank tags, set `CreatedAt = DateTime.UtcNow`, generate new `Guid` for recipe and ingredients.
- [ ] Create `UpdateRecipeUseCase`. Load by ID, return `null` if absent. Apply supplied scalar fields. When `Ingredients` is non-null, replace full ingredient list with trimmed generated rows.
- [ ] Create `GetMissingIngredientsUseCase`. Load recipe and inventory via `IRecipeRepository` + `IInventoryRepository`; return `null` when recipe absent. Normalize names with `Trim().ToUpperInvariant()`. Ignore optional ingredients. Return required ingredient names not in inventory.

### Task 1.4: Implement EF repository

- [ ] Create `RecipeRepository.cs` using `AppDbContext`.
- [ ] `GetAllAsync`: include ingredients, order by `Name`.
- [ ] `GetByIdAsync`: include ingredients, filter by ID.
- [ ] `AddAsync`: add recipe, save changes, return recipe.
- [ ] `UpdateAsync`: update recipe aggregate and save changes. Because use case mutates tracked entity from `GetByIdAsync`, implementation can call `SaveChangesAsync` and return recipe.

### Task 1.5: Add controller and DI

- [ ] Modify `Program.cs` to register:

```csharp
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<CreateRecipeUseCase>();
builder.Services.AddScoped<UpdateRecipeUseCase>();
builder.Services.AddScoped<GetMissingIngredientsUseCase>();
```

- [ ] Create `RecipesController` with `[ApiController]`, `[Route("api/recipes")]`.
- [ ] Endpoints:
  - `GET /api/recipes` → repository all → `Ok(List<RecipeDto>)`
  - `GET /api/recipes/{id:guid}` → 200 or `Problem(statusCode:404,title:"Recipe not found")`
  - `POST /api/recipes` → created `/api/recipes/{id}` or 400 `Invalid recipe payload`
  - `PATCH /api/recipes/{id:guid}` → 200 or 404/400 ProblemDetails
  - `GET /api/recipes/{id:guid}/missing` → `string[]` or 404

### Task 1.6: Verify backend

- [ ] Run: `dotnet test CookHomie.sln --filter RecipesControllerTests`
- [ ] Expected: PASS.
- [ ] Run: `dotnet test CookHomie.sln`
- [ ] Expected: PASS.

### Task 1.7: Replace Nuxt recipe mock routes with proxies

- [ ] For each recipe server route, use same proxy pattern as `server/api/inventory/index.get.ts` and `index.post.ts`.
- [ ] `index.get.ts` forwards `$fetch("/api/recipes", { baseURL: config.apiBaseUrl })`.
- [ ] `index.post.ts` reads body and forwards method `POST`.
- [ ] `[id].get.ts` forwards `/api/recipes/${id}`.
- [ ] `[id].patch.ts` reads body and forwards method `PATCH`.
- [ ] `[id]/missing.get.ts` forwards `/api/recipes/${id}/missing`.
- [ ] Delete `server/utils/recipeStore.ts` after all imports are gone.

### Task 1.8: Replace frontend recipe store tests with proxy tests

- [ ] Delete `tests/recipe-store.spec.ts` or replace its contents with route proxy tests.
- [ ] Add `tests/recipe-proxy.spec.ts` mirroring `inventory-proxy.spec.ts` for GET collection, POST, GET detail, PATCH, and GET missing.
- [ ] Stub `defineEventHandler`, `useRuntimeConfig`, `readBody`, `getRouterParam`, and `$fetch` as needed.
- [ ] Assert exact `$fetch` calls include `baseURL: "http://api:5000"` and HTTP method where needed.

### Task 1.9: Verify frontend and commit

- [ ] Run: `npm run test -- recipe-proxy`
- [ ] Expected: PASS.
- [ ] Run: `npm run validate`
- [ ] Expected: PASS.
- [ ] Commit:

```bash
git add src/CookHomie.Api src/CookHomie.Web
git commit -m "feat: persist recipes via api"
```
