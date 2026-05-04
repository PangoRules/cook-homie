# Dashboard Summary + Backend Controllers Design

**Branch:** `feat/milestone2-1-frontend-improvements`

## Problem

Dashboard expiring rows use `useDashboard()` → Nuxt `/api/dashboard/summary`. That route is still static mock data. It returns fake IDs like `inv-1`, while `/api/inventory` returns real DB UUIDs. When `pages/index.vue` opens `InventoryItemDetailModal`, it refreshes inventory and runs `items.find(i => i.id === item.id)`. Mock dashboard ID ≠ real inventory ID, so `expiringItemDetails` becomes `null`; modal receives empty prop and falls back to empty local object.

Refresh was useful diagnosis, not real fix. Root bug: dashboard summary is not backed by source-of-truth data.

Wider issue: recipes and shopping list still use Nuxt in-memory stores, while inventory is already persisted through C# API. Dashboard cannot become a correct backend-owned summary until RecipesController and ShoppingController exist.

## Current Evidence

- `src/CookHomie.Web/server/api/dashboard/summary.get.ts` has `TEMP_MOCK` and hard-coded `expiringItems`.
- `src/CookHomie.Web/server/api/recipes/*` routes use `server/utils/recipeStore.ts` in-memory mock data.
- `src/CookHomie.Web/server/api/shopping/*` routes use `server/utils/shoppingStore.ts` in-memory mock data.
- `src/CookHomie.Web/server/api/inventory/index.get.ts` already proxies C# `/api/inventory`.
- C# API currently exposes `/api/inventory` but not `/api/recipes`, `/api/shopping`, or `/api/dashboard/summary`.
- Domain entities for `Recipe`, `RecipeIngredient`, and `ShoppingItem` already exist in C# and are configured in EF Core.

## Goal

Move dashboard summary to C# API ownership by completing backend RecipesController and ShoppingController first, then adding DashboardController backed by real inventory, recipe, and shopping data. Nuxt server routes become thin proxies, not source-of-truth stores.

Success means:
- Dashboard expiring rows contain real inventory UUIDs.
- Expiring modal receives real `InventoryItem` detail and hydrates fields.
- Recipes and shopping list persist through C# API instead of Nuxt memory.
- Backend integration tests cover new controllers and dashboard summary behavior.

## Non-Goals

- No polling redesign.
- No modal visual redesign.
- No auth.
- No AI/fuzzy recipe matching.
- No schema redesign beyond repositories/use cases/controllers needed for existing entities.

## Recommended Approach

Implement backend-first verticals:

1. Full `RecipesController` + repository/use cases + integration tests.
2. Full `ShoppingController` + repository/use cases + integration tests.
3. `DashboardController` that composes inventory, recipes, and shopping queries into `/api/dashboard/summary`.
4. Nuxt proxy migration and dashboard/modal hardening.

Why this path:
- Fixes current bug at root: source-of-truth mismatch.
- Retires in-memory stores that already block real dashboard behavior.
- Aligns architecture doc: C# API owns CRUD + business rules; Nuxt proxies API; MCP/AI can consume same backend later.
- Produces branchable tasks off `feat/milestone2-1-frontend-improvements`, then merges each back into milestone branch.

## Alternatives Considered

### A. Backend-first C# controllers + dashboard endpoint

Chosen. More work, but highest leverage. Removes mocks, creates durable API contracts, enables reliable dashboard summary, and gives MCP future backend coverage.

### B. Nuxt dashboard aggregator over mixed sources

Faster bugfix, but keeps recipes/shopping in memory and delays necessary backend work. Good only as temporary patch; no longer preferred.

### C. Frontend-only ID workaround

Match by name/date/location or fetch detail on click. Fragile with duplicate items and keeps fake dashboard rows visible.

## Architecture

### C# API

Use existing controller style:
- `[ApiController]`
- `[Route("api/<resource>")]`
- `ControllerBase`
- DTO request/response types separate from EF entities
- thin controllers delegating to repositories/use cases
- `ProblemDetails` for invalid payloads and conflicts

Add repository interfaces in Domain and EF implementations in Infrastructure:
- `IRecipeRepository`
- `IShoppingRepository`

Keep business rules in Application use cases/services:
- recipe create/update validation
- shopping dedup/bulk-add behavior
- dashboard summary query composition

### Nuxt Web

Replace in-memory route handlers with proxies to C# API:
- `/api/recipes` → C# `/api/recipes`
- `/api/recipes/{id}` → C# `/api/recipes/{id}`
- `/api/recipes/{id}/missing` → C# `/api/recipes/{id}/missing`
- `/api/shopping` → C# `/api/shopping`
- `/api/shopping/bulk` → C# `/api/shopping/bulk`
- `/api/shopping/{id}` → C# `/api/shopping/{id}`
- `/api/dashboard/summary` → C# `/api/dashboard/summary`

Delete Nuxt `recipeStore.ts` and `shoppingStore.ts` once all callers proxy.

## API Surface

### Recipes

- `GET /api/recipes` returns all recipes with ingredients.
- `GET /api/recipes/{id}` returns recipe with ingredients or 404.
- `POST /api/recipes` creates recipe with ingredient rows and returns 201.
- `PATCH /api/recipes/{id}` updates scalar recipe fields and replaces ingredient list when `ingredients` supplied.
- `GET /api/recipes/{id}/missing` returns missing required ingredient names by exact normalized inventory-name matching.

Recipe matching stays MVP-simple: trim + case-insensitive exact match. Optional ingredients should not count as missing.

### Shopping

- `GET /api/shopping` returns shopping items sorted by bought status, priority, then created date/name.
- `POST /api/shopping` creates one or many items, matching existing Nuxt route behavior.
- `POST /api/shopping/bulk` accepts `{ ingredientNames: string[] }`, adds non-duplicate active items, returns 409 if all already active.
- `PATCH /api/shopping/{id}` updates partial fields, returns 404 when missing.
- `DELETE /api/shopping/{id}` deletes item, returns 404 when missing.

Dedup rule: case-insensitive name match among unbought items blocks duplicate active shopping rows. Bought historical rows should not block re-adding.

### Dashboard

- `GET /api/dashboard/summary` returns existing `DashboardSummary` shape:
  - `expiringCount`
  - `recipeMatchCount`
  - `shoppingCount`
  - `totalItems`
  - `expiringItems`
  - `recipeIdeas`

Dashboard query rules:
- `totalItems`: count inventory rows.
- `expiringItems`: inventory items with `expiresAt` today through 3 days from today, inclusive; sorted by expiry then name; includes real UUID.
- `expiringCount`: count of same expiring set.
- `shoppingCount`: count unbought shopping items.
- `recipeIdeas`: recipes with at least one required ingredient matched by current inventory; include `matchedCount` and `missingCount`; sort by highest match count, then lowest missing count, then name.
- `recipeMatchCount`: count recipes with `missingCount == 0` for required ingredients.

## Expiring Modal Behavior

`pages/index.vue` can keep using dashboard rows as lightweight summaries and resolving full item detail from `useInventory()` on click, but it must not open editable empty state when lookup fails.

Required behavior:
- remove debug console logs from dashboard page.
- remove temporary `<pre>` debug output from modal.
- when detail lookup succeeds, pass real `InventoryItem` into modal.
- when detail lookup fails, keep modal closed and show toast/error: “Item details unavailable. Refresh inventory and try again.”
- modal form must hydrate when `item` prop changes from `null` to populated item.

## Error Handling

- Invalid create/update payloads return 400 ProblemDetails.
- Duplicate active shopping item create/bulk returns 409 ProblemDetails.
- Missing recipe/shopping IDs return 404 ProblemDetails.
- Dashboard endpoint should fail clearly if dependent repository query fails; Nuxt proxy lets `usePollingFetch` surface existing dashboard error UI.
- No silent empty modal state.

## Testing

### Backend Integration Tests

Use existing `WebApplicationFactory<Program>` pattern.

Recipes:
- `GET /api/recipes` returns persisted seeded recipes with ingredients.
- `GET /api/recipes/{id}` returns 200 for known recipe and 404 for missing.
- `POST /api/recipes` persists recipe + ingredients and returns 201.
- `PATCH /api/recipes/{id}` updates recipe fields and ingredient list.
- `GET /api/recipes/{id}/missing` returns required missing ingredients using exact normalized inventory matching.

Shopping:
- `GET /api/shopping` returns persisted rows.
- `POST /api/shopping` creates single and array payloads.
- `POST /api/shopping/bulk` dedups active items and returns 409 when all names already active.
- `PATCH /api/shopping/{id}` updates fields.
- `DELETE /api/shopping/{id}` removes item and 404s when missing.

Dashboard:
- `GET /api/dashboard/summary` returns real inventory UUIDs for expiring rows.
- Expiry filter includes today through 3 days inclusive and excludes no-date/later items.
- `shoppingCount` counts unbought only.
- `recipeIdeas` and `recipeMatchCount` reflect recipe/inventory matching.

### Frontend Tests

- Nuxt API routes proxy recipes/shopping/dashboard to configured C# base URL.
- Dashboard click path does not open modal when detail lookup fails.
- `InventoryItemDetailModal` hydrates form when `item` prop changes from `null` to real item.

### Manual Validation

- Create or seed inventory item expiring within 3 days.
- Create recipe using inventory ingredients.
- Add shopping list item.
- Load dashboard.
- Confirm `/api/dashboard/summary` expiring item ID matches `/api/inventory` UUID.
- Click expiring row and confirm modal fields show real item details.
- Confirm recipe and shopping counts reflect backend data.
- Confirm no debug `<pre>` or console logs remain.

## Branching Model

Milestone branch stays `feat/milestone2-1-frontend-improvements`.

Each task below should branch from that milestone branch and merge back into it:
- `feat/m2-1-recipes-api`
- `feat/m2-1-shopping-api`
- `feat/m2-1-dashboard-api`
- `feat/m2-1-dashboard-modal-hardening`

## Tasks

- [ ] Task 1: Implement C# RecipesController, repository/use cases, Nuxt recipe proxy migration, and tests
- [ ] Task 2: Implement C# ShoppingController, repository/use cases, Nuxt shopping proxy migration, and tests
- [ ] Task 3: Implement C# DashboardController summary endpoint, Nuxt dashboard proxy migration, and tests
- [ ] Task 4: Harden dashboard expiring modal open path, fix modal prop hydration, remove debug output, and add frontend tests

## Scope Decision

Multi-task milestone expansion on existing branch `feat/milestone2-1-frontend-improvements`. Tasks are independent enough to ship as separate branches into the milestone branch, but dashboard summary should be merged after recipes and shopping APIs so it can compose real backend data.

## Spec Self-Review

- No placeholders remain.
- Design now chooses backend-first Alternative A.
- Recipes/shopping/dashboard scope is explicit and testable.
- Branching model matches requested milestone branch workflow.
- Current modal bug remains tied to real dashboard UUIDs, not polling.
