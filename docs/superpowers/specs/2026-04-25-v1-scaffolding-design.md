# CookHomie v1 Scaffolding Design

**Date:** 2026-04-25  
**Status:** Approved  
**Scope:** Full monorepo scaffolding + spike validation + first vertical slice (`add item`)

## Decisions Made

- Development style: vertical slices (end-to-end), not layer-by-layer.
- First feature: `add inventory item` for fastest full-stack validation.
- Scaffolding strategy: hybrid of approach 3 + 1.
  - Start with a short spike to validate cross-service communication.
  - Then scaffold full production structure informed by spike learnings.
  - Then implement the first end-to-end feature.

## Architecture Constraints

- C# API is a pure data/business layer (no AI reasoning).
- Python MCP server is the AI gateway and calls the C# API.
- Nuxt uses `server/api/` proxy routes to avoid CORS in local dev.
- Data model constraints preserved:
  - All IDs are UUID.
  - `RecipeIngredient.IngredientName` is text (not FK to inventory).
  - Recipe tags are PostgreSQL `text[]`.

## Delivery Phases

### Phase 1: Spike (1-2 days)

Goal: prove Docker networking and service integration before heavy scaffolding.

- Minimal C# endpoint: `GET /spike/hello`
- Minimal Nuxt page to call/display spike response
- Minimal MCP tool to call C# spike endpoint
- PostgreSQL service reachable from API

Exit criteria:

- `docker-compose up` starts all services without errors
- Web reaches API through proxy
- MCP reaches API
- No CORS or network surprises

### Phase 2: Full Scaffolding (2-3 days)

Goal: create durable foundations for v1 implementation.

- C# Clean Architecture projects:
  - `CookHomie.Domain`
  - `CookHomie.Application`
  - `CookHomie.Infrastructure`
  - `CookHomie.WebApi`
- Nuxt project structure:
  - `pages/`, `components/`, `composables/`, `layouts/`, `server/api/`, `types/`
- MCP project structure:
  - `server.py`, `api_client.py`, `tools/`, `prompts/`
- Docker + config:
  - `docker-compose.yml` and `docker-compose.dev.yml`
  - Dockerfiles for API/Web/MCP
  - `.env.example`, `.dockerignore`
- Database setup:
  - EF Core migrations initialized
  - initial schema for inventory/recipes/shopping

Exit criteria:

- `dotnet build`, `npm run build`, and Python install/typecheck pass
- Docker builds and starts all services
- Initial database schema is applied

### Phase 3: First Vertical Slice (3-4 days)

Goal: deliver `add item` from UI to DB to MCP visibility.

- API: `POST /api/inventory` with validation + persistence
- Web: `AddItemModal` form and inventory page update
- MCP: `get_inventory(location?)` tool integrated via API client
- Testing: use-case tests, component tests, end-to-end manual flow

Exit criteria:

- Add item from UI, verify persistence, verify via MCP tool

## Success Definition

- Working local stack on documented ports (3000/5000/8000/5432)
- Clear module boundaries per service
- Repeatable workflow for all future slices (recipes, shopping, matching)
- Reviewable increments with low integration risk

## Timeline

- Phase 1: 1-2 days
- Phase 2: 2-3 days
- Phase 3: 3-4 days
- Total: 6-9 days for v1 scaffolding foundation + first slice

## Notes

- This spec is the approved design baseline.
- Implementation details and granular steps are defined in the companion plan under `docs/superpowers/plans/`.
