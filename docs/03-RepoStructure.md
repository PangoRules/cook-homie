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
│   │       ├── Controllers/            # InventoryController
│   │       ├── Program.cs
│   │       └── appsettings.json
│   │   └── tests/
│   │       ├── CookHomie.Application.Tests/
│   │       ├── CookHomie.Infrastructure.Tests/
│   │       ├── CookHomie.WebApi.Tests/
│   │       └── ApiTests/
│   │           └── inventory-controller-tests.http
│   │
│   ├── CookHomie.Web/                  # Nuxt 3 frontend
│   │   ├── pages/
│   │   │   ├── index.vue               # Dashboard
│   │   │   ├── inventory.vue           # Inventory list skeleton
│   │   │   ├── recipes/
│   │   │   │   ├── index.vue           # Recipe browser skeleton
│   │   │   │   └── [id].vue            # Recipe detail
│   │   │   └── shopping.vue            # Shopping list skeleton
│   │   ├── composables/
│   │   │   ├── useInventory.ts
│   │   ├── layouts/
│   │   │   └── default.vue             # Sidebar nav
│   │   ├── server/api/                 # Nuxt proxy to C# API
│   │   │   ├── inventory/index.get.ts
│   │   │   ├── inventory/index.post.ts
│   │   ├── tests/
│   │   └── nuxt.config.ts
│   │
│   └── CookHomie.MCP/                  # Python MCP server (AI gateway)
│       ├── server.py                   # FastMCP app entry point
│       ├── api_client.py               # HTTP client for C# API
│       ├── tools/
│       │   ├── inventory.py            # get_inventory
│       │   ├── recipes.py              # get_recipes placeholder
│       │   └── shopping.py             # get_shopping_list placeholder
│       ├── prompts/
│       │   └── system.md               # CookHomie AI personality & context
│       └── pyproject.toml
│
├── docker/
│   ├── Dockerfile.api
│   ├── Dockerfile.web
│   └── Dockerfile.mcp
├── docker-compose.yml                  # Production-style local run
├── docker-compose.dev.yml              # Hot reload for dev
├── .env.example
└── README.md
```

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

Current behavior note: clients should not rely on a `Location` response header for `POST /api/inventory` yet.

Planned but not implemented yet: item-by-id lookup, update/delete, expiring-item filters, low-stock filters.

### /api/recipes (planned)
| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/recipes | List all · ?tag=quick&maxMinutes=30 |
| GET | /api/recipes/{id} | Single recipe with ingredients |
| POST | /api/recipes | Add recipe |
| PUT | /api/recipes/{id} | Update recipe |
| DELETE | /api/recipes/{id} | Remove recipe |
| GET | /api/recipes/matches | Cookable from current inventory (with match %) |
| GET | /api/recipes/{id}/missing | Missing ingredients for recipe |

### /api/shopping (planned)
| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/shopping | Current shopping list |
| POST | /api/shopping | Add item |
| PUT | /api/shopping/{id} | Update item (bought, qty) |
| DELETE | /api/shopping/{id} | Remove item |
| POST | /api/shopping/clear-bought | Remove all bought items |
| POST | /api/shopping/from-recipe/{id} | Auto-add missing ingredients |

## MCP Tools Reference

| Tool | Description |
|------|-------------|
| get_inventory(location?) | Implemented; calls the C# API and returns inventory items |
| get_recipes(query?) | Placeholder; returns an empty recipe list |
| get_shopping_list() | Placeholder; returns an empty shopping list |

Planned but not implemented yet: `get_expiring_items`, `suggest_recipes`, `get_missing_ingredients`, `build_shopping_list`, and `add_to_shopping_list`.

## 🔗 Related

- [[00-CookHomie|Overview]]
- [[01-Architecture]]
- [[02-DataModel]]
- [[04-Roadmap]]
