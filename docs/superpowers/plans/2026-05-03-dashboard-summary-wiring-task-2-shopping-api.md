# Task 2: Implement C# ShoppingController, repository/use cases, Nuxt shopping proxy migration, and tests
**Branch:** `task/m2-1-shopping-api`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-dashboard-summary-wiring-design.md`

# Shopping API Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Persist shopping list through C# API and make Nuxt shopping routes thin proxies.

**Architecture:** Domain defines shopping repository abstraction. Application owns dedup and partial update behavior. Infrastructure owns EF queries. WebApi controller returns ProblemDetails for duplicate/missing/invalid cases.

**Tech Stack:** ASP.NET Core 10 controllers, EF Core, xUnit integration tests, Nuxt 3 server routes, Vitest.

---

## Files

- Create: `src/CookHomie.Api/CookHomie.Domain/Interfaces/IShoppingRepository.cs`
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/Repositories/ShoppingRepository.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/DTOs/ShoppingDtos.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/UseCases/Shopping/CreateShoppingItemsUseCase.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/UseCases/Shopping/AddBulkShoppingItemsUseCase.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/UseCases/Shopping/UpdateShoppingItemUseCase.cs`
- Create: `src/CookHomie.Api/CookHomie.WebApi/Controllers/ShoppingController.cs`
- Modify: `src/CookHomie.Api/CookHomie.WebApi/Program.cs`
- Create: `src/CookHomie.Api/tests/CookHomie.WebApi.Tests/ShoppingControllerTests.cs`
- Modify: `src/CookHomie.Web/server/api/shopping/index.get.ts`
- Modify: `src/CookHomie.Web/server/api/shopping/index.post.ts`
- Modify: `src/CookHomie.Web/server/api/shopping/bulk.post.ts`
- Modify: `src/CookHomie.Web/server/api/shopping/[id].patch.ts`
- Modify: `src/CookHomie.Web/server/api/shopping/[id].delete.ts`
- Delete: `src/CookHomie.Web/server/utils/shoppingStore.ts`
- Create: `src/CookHomie.Web/tests/shopping-proxy.spec.ts`

## Parallelizable?

No. Dedup semantics live in Application and must be stable before controller/proxy assertions are final.

### Task 2.1: Add failing backend integration tests

- [ ] Create `ShoppingControllerTests.cs` with WebApplicationFactory setup matching inventory tests; clear `ShoppingItems`, `Recipes`, `RecipeIngredients`, `InventoryItems`; seed bought and unbought shopping rows.
- [ ] Include tests:
  - `GetShopping_ReturnsRowsSortedByBoughtPriorityCreatedName`
  - `PostShopping_WithSinglePayload_CreatesItem`
  - `PostShopping_WithArrayPayload_CreatesItems`
  - `PostShopping_WithDuplicateActiveName_ReturnsConflictProblemDetails`
  - `PostShoppingBulk_AddsOnlyNonDuplicateActiveItems`
  - `PostShoppingBulk_WhenAllAlreadyActive_ReturnsConflictProblemDetails`
  - `PatchShopping_UpdatesPartialFields`
  - `PatchShopping_WithMissingId_ReturnsNotFoundProblemDetails`
  - `DeleteShopping_RemovesItem`
  - `DeleteShopping_WithMissingId_ReturnsNotFoundProblemDetails`
- [ ] Run: `dotnet test CookHomie.sln --filter ShoppingControllerTests`
- [ ] Expected: build fails because `ShoppingController` does not exist.

### Task 2.2: Add Domain repository abstraction

- [ ] Create `IShoppingRepository`:

```csharp
using CookHomie.Domain.Entities;

namespace CookHomie.Domain.Interfaces;

public interface IShoppingRepository
{
    Task<IReadOnlyList<ShoppingItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ShoppingItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ShoppingItem?> GetActiveByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShoppingItem>> AddRangeAsync(IReadOnlyList<ShoppingItem> items, CancellationToken cancellationToken = default);
    Task<ShoppingItem?> UpdateAsync(ShoppingItem item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

### Task 2.3: Add Application DTOs and use cases

- [ ] Create DTOs: `ShoppingItemDto`, `CreateShoppingItemRequest`, `UpdateShoppingItemRequest`, `BulkShoppingRequest`.
- [ ] Represent priority in JSON as lowercase string values `low`, `medium`, `high`; map `medium` to existing Domain enum `Priority.Normal` because frontend type uses `medium` and Domain enum uses `Normal`.
- [ ] `CreateShoppingItemsUseCase`: accept one or many requests from controller, trim names, reject blank names with `ArgumentException`, reject duplicate active names with `InvalidOperationException("Active shopping item already exists.")`, allow re-add when only bought historical row exists.
- [ ] `AddBulkShoppingItemsUseCase`: accept `{ ingredientNames: string[] }`, trim nonblank unique names, skip existing unbought active names, return created items, throw `InvalidOperationException("All items already on list.")` when none created.
- [ ] `UpdateShoppingItemUseCase`: load by ID, return `null` when missing, update supplied fields only, validate duplicate active name when changing name or `IsBought` to false.

### Task 2.4: Implement EF repository

- [ ] Create `ShoppingRepository.cs`.
- [ ] `GetAllAsync` order: `IsBought` false first, `Priority.High` before `Normal` before `Low`, then `CreatedAt`, then `Name`.
- [ ] `GetActiveByNameAsync` uses `!IsBought` and case-insensitive trim match; use `ToUpper()` on both sides for EF InMemory/PostgreSQL portability.
- [ ] Implement add/update/delete with `SaveChangesAsync`.

### Task 2.5: Add controller and DI

- [ ] Modify `Program.cs` to register:

```csharp
builder.Services.AddScoped<IShoppingRepository, ShoppingRepository>();
builder.Services.AddScoped<CreateShoppingItemsUseCase>();
builder.Services.AddScoped<AddBulkShoppingItemsUseCase>();
builder.Services.AddScoped<UpdateShoppingItemUseCase>();
```

- [ ] Create `ShoppingController` with `[Route("api/shopping")]`.
- [ ] Implement endpoints:
  - `GET /api/shopping`
  - `POST /api/shopping` accepts JSON object or JSON array; controller can read `JsonElement` and deserialize to one/list.
  - `POST /api/shopping/bulk`
  - `PATCH /api/shopping/{id:guid}`
  - `DELETE /api/shopping/{id:guid}` returns `{ success = true }`
- [ ] Map validation to 400 `Invalid shopping payload`, duplicates to 409 `Duplicate shopping item`, missing IDs to 404 `Shopping item not found`.

### Task 2.6: Verify backend

- [ ] Run: `dotnet test CookHomie.sln --filter ShoppingControllerTests`
- [ ] Expected: PASS.
- [ ] Run: `dotnet test CookHomie.sln`
- [ ] Expected: PASS.

### Task 2.7: Replace Nuxt shopping routes with proxies

- [ ] Use inventory proxy pattern.
- [ ] `index.get.ts` forwards GET `/api/shopping`.
- [ ] `index.post.ts` reads body and forwards POST `/api/shopping` preserving object/array body.
- [ ] `bulk.post.ts` reads body and forwards POST `/api/shopping/bulk`.
- [ ] `[id].patch.ts` reads body and forwards PATCH `/api/shopping/${id}`.
- [ ] `[id].delete.ts` forwards DELETE `/api/shopping/${id}`.
- [ ] Delete `server/utils/shoppingStore.ts` after imports are gone.

### Task 2.8: Add frontend proxy tests

- [ ] Create `shopping-proxy.spec.ts` with tests for GET, POST, POST bulk, PATCH, DELETE.
- [ ] Stub `defineEventHandler`, `useRuntimeConfig`, `readBody`, `getRouterParam`, `$fetch`.
- [ ] Assert exact upstream URL, method, `baseURL`, body where present.

### Task 2.9: Verify frontend and commit

- [ ] Run: `npm run test -- shopping-proxy`
- [ ] Expected: PASS.
- [ ] Run: `npm run validate`
- [ ] Expected: PASS.
- [ ] Commit:

```bash
git add src/CookHomie.Api src/CookHomie.Web
git commit -m "feat: persist shopping list via api"
```
