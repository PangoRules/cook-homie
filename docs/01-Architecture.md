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

### Frontend styling — Tailwind v4
Styling uses Tailwind CSS v4 via `@tailwindcss/vite`. A single file (`assets/css/main.css`) owns the entire style layer:

- **`@import "tailwindcss"`** — Tailwind base, preflight, and utility generation.
- **`@theme {}`** — custom design tokens: the "Calm Utility" color palette, `font-display`/`font-body`/`font-mono`, and border radii. These generate first-class utilities (`bg-accent`, `text-text-primary`, `font-display`, `rounded-md`, etc.). Spacing, shadows, and sizing use Tailwind defaults.
- **`@layer base {}`** — global element resets and typography defaults.
- **`@layer components {}`** — shared multi-element classes: `.btn`, `.btn-primary`, `.btn-secondary`, `.card`, `.input`, `.skeleton`. Used in templates via class attribute; no scoped `<style>` blocks exist in any component.

No `tailwind.config.js` is used — Tailwind v4 is fully CSS-first.

### DRY
All items must follow the principle. If a UI component uses more than 3 classes and it's used in 2 or more places across the UI, it must be extracted into a single shared component in `components/shared/` and used consistently everywhere. The same applies to all code layers — duplicated logic in MCP tools, C# application/infrastructure classes, or composables must be pulled into shared, reusable units rather than copy-pasted.

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
