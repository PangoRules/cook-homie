# Task 3: Implement C# DashboardController summary endpoint, Nuxt dashboard proxy migration, and tests
**Branch:** `task/m2-1-dashboard-api`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-dashboard-summary-wiring-design.md`

# Dashboard API Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Serve `/api/dashboard/summary` from C# using real inventory, recipe, and shopping data, then proxy Nuxt dashboard route to it.

**Architecture:** Dashboard is Application query composition over repository abstractions. Controller stays thin. Nuxt route stops owning summary data.

**Tech Stack:** ASP.NET Core 10 controllers, EF Core repositories from Tasks 1-2, xUnit integration tests, Nuxt 3 server route, Vitest.

---

## Files

- Create: `src/CookHomie.Api/CookHomie.Application/DTOs/DashboardDtos.cs`
- Create: `src/CookHomie.Api/CookHomie.Application/UseCases/Dashboard/GetDashboardSummaryUseCase.cs`
- Create: `src/CookHomie.Api/CookHomie.WebApi/Controllers/DashboardController.cs`
- Modify: `src/CookHomie.Api/CookHomie.WebApi/Program.cs`
- Modify: `src/CookHomie.Api/CookHomie.Domain/Interfaces/IInventoryRepository.cs`
- Modify: `src/CookHomie.Api/CookHomie.Infrastructure/Repositories/InventoryRepository.cs`
- Create: `src/CookHomie.Api/tests/CookHomie.WebApi.Tests/DashboardControllerTests.cs`
- Modify: `src/CookHomie.Web/server/api/dashboard/summary.get.ts`
- Create: `src/CookHomie.Web/tests/dashboard-proxy.spec.ts`
- Modify: `src/CookHomie.Web/tests/server-mock-audit.spec.ts` only if assertion must also fail on leftover dashboard mocks; do not weaken TEMP_MOCK audit.

## Parallelizable?

Partly. After DTO field names are fixed, Nuxt dashboard proxy test/route can be implemented in parallel with backend query internals. Backend tests require Tasks 1 and 2 merged first.

### Task 3.1: Confirm prerequisites

- [ ] Ensure task branch includes merged Tasks 1 and 2: `IRecipeRepository`, `IShoppingRepository`, `RecipeRepository`, `ShoppingRepository`, and their DI registrations exist.
- [ ] If missing, stop and merge task branches into `feat/milestone2-1-frontend-improvements` before implementing dashboard.

### Task 3.2: Add failing backend integration tests

- [ ] Create `DashboardControllerTests.cs` with WebApplicationFactory setup matching existing controller tests.
- [ ] Seed inventory items:
  - one expires today
  - one expires in 3 days
  - one expires in 4 days
  - one with `ExpiresAt = null`
- [ ] Seed shopping items: one unbought, one bought.
- [ ] Seed recipes:
  - fully matched required ingredients
  - partially matched required ingredients
  - optional missing ingredient only
- [ ] Include tests:
  - `GetDashboardSummary_ReturnsRealInventoryUuidsForExpiringRows`
  - `GetDashboardSummary_ExpiryWindowIncludesTodayThroughThreeDaysOnly`
  - `GetDashboardSummary_ShoppingCountCountsUnboughtOnly`
  - `GetDashboardSummary_RecipeIdeasAndMatchCountReflectRequiredIngredientMatches`
- [ ] Run: `dotnet test CookHomie.sln --filter DashboardControllerTests`
- [ ] Expected: build fails because `DashboardController` does not exist.

### Task 3.3: Add inventory repository query support

- [ ] Extend `IInventoryRepository` with count/window methods:

```csharp
Task<int> CountAsync(CancellationToken cancellationToken = default);
Task<IReadOnlyList<InventoryItem>> GetExpiringBetweenAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken = default);
```

- [ ] Implement in `InventoryRepository`:
  - `CountAsync` uses `_context.InventoryItems.CountAsync(cancellationToken)`.
  - `GetExpiringBetweenAsync` filters `ExpiresAt.HasValue && ExpiresAt.Value >= start && ExpiresAt.Value <= end`, orders by `ExpiresAt`, then `Name`.
- [ ] Update any fake `IInventoryRepository` in Application tests with methods returning zero/empty list.

### Task 3.4: Add Dashboard DTOs and use case

- [ ] Create DTOs with JSON-compatible property names by C# default camelCase policy:

```csharp
public sealed class DashboardSummaryDto
{
    public int ExpiringCount { get; set; }
    public int RecipeMatchCount { get; set; }
    public int ShoppingCount { get; set; }
    public int TotalItems { get; set; }
    public IReadOnlyList<DashboardExpiringItemDto> ExpiringItems { get; set; } = [];
    public IReadOnlyList<DashboardRecipeIdeaDto> RecipeIdeas { get; set; } = [];
}
```

- [ ] Add `DashboardExpiringItemDto` fields: `Guid Id`, `string Name`, `DateOnly ExpiresAt`, `string Location`.
- [ ] Add `DashboardRecipeIdeaDto` fields: `Guid Id`, `string Name`, `int MatchedCount`, `int MissingCount`.
- [ ] Create `GetDashboardSummaryUseCase` depending on `IInventoryRepository`, `IRecipeRepository`, `IShoppingRepository`.
- [ ] Use `DateOnly.FromDateTime(DateTime.UtcNow)` for today; end = today.AddDays(3).
- [ ] Recipe logic: for each recipe, consider only `Ingredients.Where(i => !i.IsOptional)`. Match by `Trim().ToUpperInvariant()` against all inventory names. Include recipe idea if `matchedCount > 0`. Sort by `matchedCount desc`, `missingCount asc`, `Name asc`. `RecipeMatchCount` is count where required ingredient count > 0 and `missingCount == 0`.
- [ ] Shopping count: count `shoppingItems.Count(i => !i.IsBought)` from repository all if `IShoppingRepository` lacks a count method; do not add DB-specific shortcut unless needed.

### Task 3.5: Add controller and DI

- [ ] Modify `Program.cs`:

```csharp
builder.Services.AddScoped<GetDashboardSummaryUseCase>();
```

- [ ] Create `DashboardController`:
  - `[ApiController]`
  - `[Route("api/dashboard")]`
  - `GET summary` calls use case and returns `Ok(summary)`.
- [ ] Do not catch repository exceptions; global exception handler should produce clear failure. Do catch `ArgumentException` only if added by use case validation.

### Task 3.6: Verify backend

- [ ] Run: `dotnet test CookHomie.sln --filter DashboardControllerTests`
- [ ] Expected: PASS.
- [ ] Run: `dotnet test CookHomie.sln`
- [ ] Expected: PASS.

### Task 3.7: Replace Nuxt dashboard mock route with proxy

- [ ] Replace `server/api/dashboard/summary.get.ts` with proxy:

```ts
import type { DashboardSummary } from "~/types";

export default defineEventHandler(async (): Promise<DashboardSummary> => {
  const config = useRuntimeConfig();
  return await $fetch<DashboardSummary>("/api/dashboard/summary", {
    baseURL: config.apiBaseUrl,
  });
});
```

- [ ] Remove all `TEMP_MOCK`, fake IDs, fake counts.

### Task 3.8: Add frontend dashboard proxy test

- [ ] Create `dashboard-proxy.spec.ts` mirroring inventory proxy GET test.
- [ ] Assert handler calls `$fetch("/api/dashboard/summary", { baseURL: "http://api:5000" })` and resolves upstream payload unchanged.
- [ ] Keep `server-mock-audit.spec.ts` passing; if dashboard fake markers are gone, no change is required.

### Task 3.9: Verify frontend and commit

- [ ] Run: `npm run test -- dashboard-proxy server-mock-audit`
- [ ] Expected: PASS.
- [ ] Run: `npm run validate`
- [ ] Expected: PASS.
- [ ] Commit:

```bash
git add src/CookHomie.Api src/CookHomie.Web
git commit -m "feat: serve dashboard summary from api"
```
