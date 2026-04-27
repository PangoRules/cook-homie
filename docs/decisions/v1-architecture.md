# CookHomie v1 Architecture Decision

**Date:** 2026-04-25
**Status:** Implemented

## Decisions Made

- Development style: vertical slices (end-to-end), not layer-by-layer.
- First feature: `add inventory item` for fastest full-stack validation.
- Scaffolding strategy: start with a short spike to validate cross-service communication, then scaffold full production structure, then implement the first end-to-end feature.

## Architecture Constraints

- C# API is a pure data/business layer (no AI reasoning).
- Python MCP server is the AI gateway and calls the C# API.
- Nuxt uses `server/api/` proxy routes to avoid CORS in local dev.
- Data model constraints:
  - All IDs are UUID.
  - `RecipeIngredient.IngredientName` is text (not FK to inventory).
  - Recipe tags are PostgreSQL `text[]`.

## Delivery Phases

### Phase 1: Spike
Goal: prove Docker networking and service integration before heavy scaffolding.

- Minimal C# endpoint: `GET /spike/hello`
- Minimal Nuxt page to call/display spike response
- Minimal MCP tool to call C# spike endpoint
- PostgreSQL service reachable from API

Exit criteria: all services reachable, no CORS or network surprises.

> Spike code was retired after Phase 2 validation. `CookHomie.SpikeApi` no longer exists in the codebase.

### Phase 2: Full Scaffolding
Goal: create durable foundations for v1 implementation.

- C# Clean Architecture projects: Domain / Application / Infrastructure / WebApi
- Nuxt project structure: pages, components, composables, layouts, server/api, types
- MCP project structure: server.py, api_client.py, tools, prompts
- Docker + config: docker-compose.yml and docker-compose.dev.yml, Dockerfiles, .env.example
- EF Core migrations initialized, initial schema for inventory/recipes/shopping

### Phase 3: First Vertical Slice
Goal: deliver `add item` from UI to DB to MCP visibility.

- API: `POST /api/inventory` with validation + persistence
- Web: `AddItemModal` form and inventory page
- MCP: `get_inventory(location?)` tool integrated via API client
- E2E smoke script and CI workflow

## Success Definition

- Working local stack on documented ports (3000/5000/8000/5432)
- Clear module boundaries per service
- Repeatable workflow for future slices (recipes, shopping, matching)
