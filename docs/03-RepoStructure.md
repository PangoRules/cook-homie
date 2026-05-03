---
title: CookHomie Repo Structure
tags: [project, cookhomie, structure]
---

> **Navigation:** [[00-CookHomie|Overview]] → [[01-Architecture]] → [[02-DataModel]] → 03 RepoStructure → [[04-Roadmap]]

## Full Monorepo Layout

```
CookHomie/
├── src/
│   ├── CookHomie.Api/                  # C# solution (Clean Architecture)
│   │   ├── CookHomie.Domain/
│   │   │   ├── Entities/               # InventoryItem, Recipe, RecipeIngredient, ShoppingItem
│   │   │   ├── Enums/                  # Location, Priority
│   │   │   └── Interfaces/             # IInventoryRepository
│   │   ├── CookHomie.Application/
│   │   │   ├── UseCases/               # Inventory use cases
│   │   │   └── DTOs/                   # Request/Response objects
│   │   ├── CookHomie.Infrastructure/
│   │   │   ├── Persistence/            # AppDbContext (EF Core)
│   │   │   ├── Repositories/           # InventoryRepository
│   │   │   └── Migrations/
│   │   ├── CookHomie.WebApi/
│   │   │   ├── Controllers/            # InventoryController
│   │   │   ├── Program.cs
│   │   │   └── appsettings.json
│   │   └── tests/
│   │       ├── CookHomie.Application.Tests/
│   │       ├── CookHomie.Infrastructure.Tests/
│   │       ├── CookHomie.WebApi.Tests/
│   │       └── ApiTests/
│   │           └── inventory-controller-tests.http
│   │
│   ├── CookHomie.Web/                  # Nuxt 3 frontend
│   │   ├── pages/
│   │   │   ├── index.vue               # Dashboard (Kitchen Overview)
│   │   │   ├── inventory.vue           # Inventory list
│   │   │   ├── shopping.vue            # Shopping list
│   │   │   ├── development.vue         # Dev-mode polling playground
│   │   │   └── recipes/
│   │   │       ├── index.vue           # Recipe list (via /recipes route)
│   │   │       └── [id].vue            # Recipe detail with in-stock highlighting
│   │   ├── assets/css/
│   │   │   └── main.css               # Tailwind v4 entry: @theme tokens, @layer base/components
│   │   ├── components/
│   │   │   ├── dashboard/             # Auto-import: <DashboardXxx /> (prefix deduplicated)
│   │   │   │   ├── DashboardStatCard.vue  → <DashboardStatCard />
│   │   │   │   └── DashboardPanel.vue    → <DashboardPanel />
│   │   │   ├── inventory/             # Auto-import: <InventoryXxx />
│   │   │   │   └── AddItemModal.vue      → <InventoryAddItemModal />
│   │   │   ├── recipes/               # Auto-import: <RecipesXxx />
│   │   │   │   ├── AddRecipeModal.vue    → <RecipesAddRecipeModal />
│   │   │   │   ├── RecipeCard.vue        → <RecipesRecipeCard />
│   │   │   │   └── detail/
│   │   │   │       └── IngredientStockBadge.vue  → <RecipesDetailIngredientStockBadge />
│   │   │   └── shared/                # Auto-import: <SharedXxx />
│   │   │       ├── Button.vue            → <SharedButton />
│   │   │       ├── DevModeGuard.vue      → <SharedDevModeGuard />
│   │   │       ├── ErrorBanner.vue       → <SharedErrorBanner />
│   │   │       ├── FormField.vue         → <SharedFormField />
│   │   │       ├── Modal.vue             → <SharedModal />
│   │   │       ├── NavLink.vue           → <SharedNavLink />
│   │   │       ├── SkeletonBlock.vue     → <SharedSkeletonBlock />
│   │   │       ├── StaleIndicator.vue    → <SharedStaleIndicator />
│   │   │       ├── TagInput.vue          → <SharedTagInput />
│   │   │       └── ToastContainer.vue    → <SharedToastContainer />
│   │   ├── composables/
│   │   │   ├── useDashboard.ts        # Dashboard composable (wraps usePollingFetch)
│   │   │   ├── useDevMode.ts          # Dev mode state
│   │   │   ├── useInventory.ts        # Inventory list + add-item
│   │   │   ├── useLocale.ts           # i18n/locale
│   │   │   ├── usePollingFetch.ts     # Generic 30s polling composable
│   │   │   ├── useRecipeDetail.ts     # Recipe detail + in-stock enrichment
│   │   │   ├── useRecipes.ts          # Recipe list
│   │   │   ├── useShoppingList.ts     # Shopping list + add-item
│   │   │   └── useToast.ts            # Toast notifications
│   │   ├── layouts/
│   │   │   └── default.vue            # App shell: header nav + <slot> + toast
│   │   ├── server/api/                # Nuxt proxy to C# API (and mock fallbacks)
│   │   │   ├── dashboard/
│   │   │   │   └── summary.get.ts     # Mock: returns hardcoded summary counts
│   │   │   ├── dev/
│   │   │   │   └── mode.get.ts        # Returns { env, ts }
│   │   │   ├── hello.get.ts
│   │   │   ├── inventory/
│   │   │   │   ├── index.get.ts       # Proxies to C# /api/inventory
│   │   │   │   └── index.post.ts      # Proxies to C# /api/inventory
│   │   │   ├── recipes/
│   │   │   │   ├── index.get.ts       # Mock: reads from recipeStore
│   │   │   │   ├── index.post.ts      # Mock: writes to recipeStore
│   │   │   │   ├── [id].get.ts        # Mock: reads from recipeStore
│   │   │   │   └── [id]/
│   │   │   │       └── missing.get.ts # Mock: returns empty (stock matching client-side)
│   │   │   └── shopping/
│   │   │       └── (not yet wired — shopping list not connected to C# API)
│   │   ├── server/utils/
│   │   │   └── recipeStore.ts         # In-memory mock store; delete when C# RecipesController ships
│   │   ├── types/
│   │   │   └── index.ts              # Shared TypeScript interfaces
│   │   ├── utils/
│   │   │   ├── date.ts
│   │   │   └── ingredientStock.ts    # Client-side exact normalized ingredient matching
│   │   └── nuxt.config.ts
│   │
│   └── CookHomie.MCP/                  # Python MCP server (AI gateway)
│       ├── server.py                   # FastMCP app entry point (Starlette + FastMCP on port 8000)
│       ├── api_client.py               # HTTP client for C# API (reads API_BASE_URL / COOKHOMIE_API_BASE_URL)
│       ├── tools/
│       │   ├── inventory.py           # get_inventory — calls C# API ✅
│       │   ├── recipes.py              # get_recipes — placeholder (returns empty)
│       │   └── shopping.py            # get_shopping_list — placeholder (returns empty)
│       ├── prompts/
│       │   └── system.md              # CookHomie AI personality & context
│       ├── tests/
│       │   ├── test_inventory_tool.py
│       │   └── test_server_bootstrap.py
│       └── pyproject.toml
│
├── docker/
│   ├── Dockerfile.api
│   ├── Dockerfile.web
│   └── Dockerfile.mcp
├── docker-compose.yml                  # Production-style local run (postgres + api + ui profile)
├── .env.example
└── README.md
```

## Nuxt Component Auto-Import Naming

Nuxt generates component names from their path relative to `components/`:

```
components/<Folder>/<Name>.vue  →  <FolderName />
```

**Deduplication rule:** if the filename already starts with the folder name (case-insensitive), the folder prefix is dropped.

```
dashboard/DashboardPanel.vue  →  <DashboardPanel />      ✅ (not DashboardDashboardPanel)
shared/StaleIndicator.vue     →  <SharedStaleIndicator /> ✅ ("Shared" prefix added)
```

### Full reference

| File | Use in templates as |
|------|-------------------|
| `shared/Button.vue` | `<SharedButton />` |
| `shared/DevModeGuard.vue` | `<SharedDevModeGuard />` |
| `shared/ErrorBanner.vue` | `<SharedErrorBanner />` |
| `shared/FormField.vue` | `<SharedFormField />` |
| `shared/Modal.vue` | `<SharedModal />` |
| `shared/NavLink.vue` | `<SharedNavLink />` |
| `shared/SkeletonBlock.vue` | `<SharedSkeletonBlock />` |
| `shared/StaleIndicator.vue` | `<SharedStaleIndicator />` |
| `shared/TagInput.vue` | `<SharedTagInput />` |
| `shared/ToastContainer.vue` | `<SharedToastContainer />` |
| `dashboard/DashboardPanel.vue` | `<DashboardPanel />` |
| `dashboard/DashboardStatCard.vue` | `<DashboardStatCard />` |
| `inventory/AddItemModal.vue` | `<InventoryAddItemModal />` |
| `recipes/AddRecipeModal.vue` | `<RecipesAddRecipeModal />` |
| `recipes/RecipeCard.vue` | `<RecipesRecipeCard />` |
| `recipes/detail/IngredientStockBadge.vue` | `<RecipesDetailIngredientStockBadge />` |

---

## API Endpoints Reference

### /api/inventory
| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/inventory | List all inventory items |
| POST | /api/inventory | Create inventory item via application use case; returns `201` with created item body |

`POST /api/inventory` request fields currently expected by the API:

| Field | Required | Notes |
|------|----------|-------|
| `name` | Yes | Non-empty |
| `category` | Yes | Non-empty |
| `location` | Yes | `Pantry`, `Fridge`, `Freezer`, or `Spices` |
| `quantity` | Yes | Must be greater than `0` |
| `unit` | Yes | Non-empty |
| `expiresAt` | No | Optional date |
| `isOpened` | No | Boolean |
| `notes` | No | Optional text |

Validation failures return `400` with `application/problem+json`.

### /api/recipes
**Status:** Nuxt server uses an in-memory mock store (`server/utils/recipeStore.ts`) — no C# RecipesController exists yet. The mock ships with 3 hardcoded recipes (Pancakes, Tomato Pasta, Avocado Toast).

| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/recipes | List all recipes (mock store) |
| POST | /api/recipes | Add recipe (mock store, no C# backend) |
| GET | /api/recipes/{id} | Single recipe (mock store) |
| GET | /api/recipes/{id}/missing | Missing ingredients (client-side matching, returns empty) |

### /api/shopping
**Status:** Shopping list page is wired to `/api/shopping` via `useShoppingList` composable. No C# ShoppingController exists and no Nuxt server route is currently created, so the endpoint is not backed by an implementation.

| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/shopping | Current shopping list |
| POST | /api/shopping | Add item |

### /api/polling-data
**Status:** Dev-only endpoint exposed by `PollingController` in C# API. Returns simulated data `{ timestamp, itemsCount, lastPolled }`. Used exclusively by the `/development` page for polling demos.

### /api/dashboard/summary (web proxy)
**Status:** Mock — returns hardcoded `{ expiringCount: 3, recipeMatchCount: 7, shoppingCount: 4 }`. Replace with C# DashboardController when it ships.

### /api/dev (web only)
| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/dev/mode | Get dev mode status (`{ env, ts }`) |

---

## MCP Tools Reference

All three tools are registered with FastMCP, but only `get_inventory` calls the real C# API:

| Tool | Implementation | Status |
|------|---------------|--------|
| `get_inventory(location?)` | Calls `ApiClient.get_inventory()` → C# `/api/inventory` | **Functional** ✅ |
| `get_recipes(query?)` | Returns `{"recipes": []}` (placeholder) | Stub — needs C# RecipesController |
| `get_shopping_list()` | Returns `{"items": []}` (placeholder) | Stub — needs C# ShoppingController |

Planned but not implemented: `get_expiring_items`, `suggest_recipes`, `get_missing_ingredients`, `build_shopping_list`, `add_to_shopping_list`.

The MCP server also hosts HTTP endpoints on the same port (Starlette routes `/api/inventory`, `/api/recipes`, `/api/shopping-list`) alongside the FastMCP mount at `/mcp`.

---

## 🔗 Related

- [[00-CookHomie|Overview]]
- [[01-Architecture]]
- [[02-DataModel]]
- [[04-Roadmap]]
