# CookHomie Development Guide

This repository is the food and kitchen module of HomieOS, a local-first web app for kitchen inventory management, recipe suggestions, and shopping lists, exposing all functionality via MCP tools for AI integration.

## Project Status
- **MVP Phase**: Core verticals (inventory, recipes, shopping list) implemented. Frontend milestone 2 complete.
- **Monorepo Structure**: 3 services: API (C# Clean Architecture), Web (Nuxt 3), MCP (Python FastMCP).
- **Development Focus**: Iterating on UX, completing remaining milestone tasks, expanding MCP tool coverage.

## Monorepo Structure
```
src/
├── CookHomie.Api/                  # C# ASP.NET Core 10 (Clean Architecture)
│   ├── CookHomie.Domain/           # Entities, enums, interfaces
│   ├── CookHomie.Application/      # Use cases, DTOs, interfaces
│   ├── CookHomie.Infrastructure/   # EF Core, repositories, migrations
│   └── CookHomie.WebApi/           # Controllers, DI wiring
│
├── CookHomie.Web/                  # Nuxt 3 frontend
│   ├── pages/                      # UI pages
│   ├── components/                 # Vue components (not fully built yet)
│   ├── composables/                # Shared logic
│   └── server/api/                 # Nuxt proxy to C# API
│
└── CookHomie.MCP/                  # Python FastMCP server
    ├── server.py                   # FastMCP app entry point
    ├── api_client.py               # HTTP client for C# API
    ├── tools/                      # MCP tools: inventory, recipes, shopping
    └── prompts/                    # System prompt for AI personality
```

## Architecture Principles
1. **API is pure data layer**: No AI logic. Handles CRUD + business rules (recipe matching, expiry alerts).
2. **MCP server is AI gateway**: Calls C# API for data, adds AI reasoning on top. Provider-agnostic (Claude, Ollama, etc.).
3. **Name-based ingredient matching**: RecipeIngredient stores `IngredientName` as text, avoiding FK to inventory for fuzzy matching.
4. **Clean Architecture**: C# layers (Domain → Application → Infrastructure → WebApi)

## Development Environment
- **Service Ports** (local dev):
  - Nuxt Web: 3000
  - C# API: 5000
  - MCP Server: 8000
  - PostgreSQL: 5432
- **Stack**: Nuxt 3 + Vue 3 | ASP.NET Core 10 + EF Core | PostgreSQL | Python FastMCP | Docker Compose

## Key Design Decisions
- **All IDs are UUIDs**: For future multi-user or sync scenarios
- **Tags as PostgreSQL arrays**: Avoid joins for MVP, filterable with `ANY()` queries
- **No auth for MVP**: Single user, local-only
- **Nuxt server/api proxy**: Avoids CORS in local development

## Validation — MANDATORY Before Any PR

Run these against whichever layer you touched. All must pass before signalling done or handing off to the git agent.

### Frontend — `src/CookHomie.Web/`

```bash
cd src/CookHomie.Web

npm run validate        # typecheck + lint + format:check + build in one shot (use this)

# individually if you need to isolate a failure:
npm run typecheck       # vue-tsc — catches type errors across .vue and .ts
npm run lint            # eslint — catches style and correctness issues
npm run format:check    # prettier — check formatting without fixing
npm run format          # prettier — fix formatting in place
npm run build           # nuxt build — proves the app bundles without errors
npm run test            # vitest — runs unit tests
```

### Backend — `src/CookHomie.Api/`

```bash
cd src/CookHomie.Api

dotnet build CookHomie.sln          # compile — catches type and reference errors
dotnet test CookHomie.sln           # run all test projects
```

### MCP Server — `src/CookHomie.MCP/`

```bash
cd src/CookHomie.MCP
python -m py_compile server.py api_client.py   # syntax check
pytest tests/                                  # unit tests
```

### Rules

- Run only the layers you touched — no need to run all three for a frontend-only change.
- If `npm run validate` fails, fix it before committing. Do not commit with known lint or type errors.
- Formatting failures: run `npm run format` to auto-fix, then re-run `npm run format:check` to confirm clean.
- Test failures are blockers. Do not hand off to the git agent with failing tests.
- **Before any PR**: `npm run validate` must pass — this now includes prettier format check.
- A pre-push git hook enforces `format:check` automatically when frontend files changed.

## Documentation
Full architecture, data model, and roadmap documentation in the `docs/` directory:
- [[00-CookHomie]]
- [[01-Architecture]]
- [[02-DataModel]]
- [[03-RepoStructure]]
- [[04-Roadmap]]
