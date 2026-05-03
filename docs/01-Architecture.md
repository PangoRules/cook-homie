---
title: CookHomie Architecture
tags: [project, cookhomie, architecture]
---

> **Navigation:** [[00-CookHomie|Overview]] → 01 Architecture → [[02-DataModel]] → [[03-RepoStructure]] → [[04-Roadmap]]

## System Architecture

CookHomie uses a monorepo with three services communicating over HTTP locally, all orchestrated by Docker Compose.

```
┌─────────────────┐     ┌──────────────────────┐
│   Nuxt 3 Web    │     │  Claude Code / opencode│
│   (Browser UI)  │     │  (any MCP client)     │
└────────┬────────┘     └──────────┬────────────┘
         │ REST                    │ MCP Protocol
         ▼                         ▼
┌─────────────────┐     ┌──────────────────────┐
│  C# ASP.NET API │     │  Python MCP Server   │
│  Clean Arch     │◄────│  (AI Gateway)        │
│  Business logic │ HTTP└──────────────────────┘
└────────┬────────┘
         │ EF Core
         ▼
┌─────────────────┐     ┌──────────────────────┐
│   PostgreSQL    │     │   AI Providers       │
│   (local Docker)│     │  Claude / Ollama /   │
└─────────────────┘     │  any MCP-compatible  │
                        └──────────────────────┘
```

## Current Runtime State

- `docker-compose.yml` builds the API container from `src/CookHomie.Api/CookHomie.WebApi` and runs `CookHomie.WebApi.dll`.
- WebApi exposes `/api/inventory`, `/health`, and development OpenAPI/Swagger routes.
- Inventory item creation flows through `AddInventoryItemUseCase` (application layer) instead of direct controller-to-repository writes.
- WebApi validation errors for inventory create requests return RFC7807 Problem Details responses.
- Nuxt proxies inventory calls through `src/CookHomie.Web/server/api/inventory`.
- MCP registers inventory, recipe, and shopping placeholder tools; only inventory currently calls the C# API.

## Design Decisions

### C# API is pure data
No AI logic in the API layer. It handles CRUD, business rules, and smart queries (recipe matching, expiry alerts). AI reasoning lives exclusively in the MCP server.

### MCP Server is the AI gateway
The Python MCP server calls the C# API for data, then adds AI reasoning on top. Provider-agnostic by design — whoever runs the MCP client picks the AI model (Claude, Ollama, Gemini, etc.).

### Clean Architecture (C# side)
- **Domain** — entities, interfaces, enums. No dependencies.
- **Application** — use cases, DTOs, service interfaces. Depends on Domain only.
- **Infrastructure** — EF Core, PostgreSQL repos. Depends on Application + Domain.
- **WebApi** — controllers, DI wiring, health checks, OpenAPI. Depends on Application + Infrastructure.

### No auth for MVP
Local-only, single user. API key or JWT added in v2 when exposing beyond localhost.

### Nuxt server/api proxy
Nuxt routes frontend requests through its own `server/api/` layer to the C# API. Avoids CORS issues in local dev and keeps the API base URL in one place.

### Frontend styling — Tailwind v4, no scoped styles
All styling lives in `assets/css/main.css`. There are **zero scoped `<style>` blocks** in any Vue file — every visual element uses either inline Tailwind utility classes or shared `@layer components` classes from main.css.

**`assets/css/main.css` structure:**

```
@import "tailwindcss"          ← generates all utility classes
  @theme { }                  ← design tokens → bg-*, text-*, font-*, rounded-*
  @keyframes shimmer { }      ← skeleton loader animation
  @keyframes modalIn/Out { }  ← Modal open/close animations
  @keyframes backdropIn/Out {}
  @layer base { }              ← resets, body defaults, headings, links
  @layer components { }        ← all shared multi-element patterns
```

**Shared component classes (`@layer components`):**

| Class | Purpose |
|-------|---------|
| `.btn` | Base button (inline-flex, padding, rounded, cursor) |
| `.btn-primary` | Accent background, white text, hover state, disabled |
| `.btn-secondary` | Transparent with border, hover surface |
| `.input` | Full-width text input, border, focus ring on accent |
| `.card` | Surface background, border, rounded, shadow-sm |
| `.skeleton` | Shimmer loading block (skeleton loader) |
| `.tag-chip` | Accent-subtle pill with uppercase label text |
| `.toast-enter/leave` | Toast notification slide-in animation |

**Rule: when to extract to `@layer components`**
If a multi-element pattern (button + label + icon, card header + body + footer, etc.) is used in 2+ places and totals 4+ utility classes, it belongs in `@layer components`, not inline. Single-element utilities stay inline (e.g., `class="text-sm text-text-muted"`).

**Rule: no scoped `<style>`**
Scoped styles create unmaintainable CSS islands. If you need a one-off style that can't be inline or shared, extract it to a component. If it genuinely can't be extracted, it's a signal something needs refactoring.

The wiring: `nuxt.config.ts` loads `@tailwindcss/vite` (line 26) and `css: ["~/assets/css/main.css"]` (line 34). Both are required for every page.

### DRY — extract, don't duplicate

Every layer enforces DRY. When code is copied rather than shared, it becomes inconsistent and unmaintainable as it diverges across copies.

**Frontend (Vue/Nuxt):**
- Shared multi-element UI patterns → `components/shared/`. Examples: `ErrorBanner`, `SkeletonBlock`, `Modal`, `ToastContainer`, `FormField`, `Button`, `StaleIndicator`.
- Composable logic (data fetching, polling, state) → `composables/`. If two pages do the same data operation, share the composable, don't copy the fetch logic.
- Repeated inline utility patterns (4+ classes used in 2+ places) → extract to `@layer components` in `main.css`.

**Backend (C#):**
- Shared business logic → `CookHomie.Application/` use cases, not copied into controllers.
- Repository interfaces → `CookHomie.Domain/Interfaces/`. Implementations in `CookHomie.Infrastructure/Repositories/`.
- Duplicated query logic or validation → belongs in a use case or a shared service, not in a controller.

**MCP server (Python):**
- HTTP logic → `ApiClient` class. Not copied across tools.
- Tool functions should delegate to `ApiClient` methods; don't re-implement HTTP calls in each tool.

**Threshold:** if a pattern appears in 2+ places, it must be extracted. The only exception is trivial one-liners (single utility class, simple prop pass-through) where extraction adds more noise than it removes.

**Consequence of violation:** copied logic that diverges is a bug. PRs that introduce duplication without extracting first should be flagged in review.

### Client-side ingredient stock matching
Recipe detail page (`recipes/[id].vue`) does not call a backend to determine which ingredients are in stock. Instead, `useRecipeDetail` composable reads inventory names from the shared `useInventory` state and uses `utils/ingredientStock.ts` for exact normalized matching: trim both names and compare case-insensitively. This is a deliberate MVP simplification — alias maps, pluralization, substring matching, and AI-powered fuzzy matching are later work.

### Nuxt component auto-import naming
Nuxt auto-imports all components from `components/` using their folder path as a prefix:

```
components/<Folder>/<Name>.vue → <FolderName />
```

**Exception (deduplication):** if the filename already starts with the folder name, the prefix is dropped — `dashboard/DashboardPanel.vue` → `<DashboardPanel />` not `<DashboardDashboardPanel />`.

See `03-RepoStructure.md` for the full reference table.

## Service Ports (local dev)

| Service | Port |
|---------|------|
| Nuxt Web | 3000 |
| C# API | 5000 |
| MCP Server | 8000 |
| PostgreSQL | 5432 |

## 🔗 Related

- [[00-CookHomie|Overview]]
- [[02-DataModel]]
- [[03-RepoStructure]]
