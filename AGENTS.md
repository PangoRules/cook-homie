# CookHomie Development Guide

This repository is the food and kitchen module of HomieOS, a local-first web app for kitchen inventory management, recipe suggestions, and shopping lists, exposing all functionality via MCP tools for AI integration.

## Project Status
- **MVP Phase**: Early development with architecture documented but no source code implemented
- **Monorepo Structure**: Planned with 3 services (API, Web, MCP server) but not yet scaffolded
- **Development Focus**: Implementation of C# clean architecture, Nuxt frontend, and Python MCP server

## Monorepo Structure (Planned)
```
src/
├── CookHomie.Api/                  # C# ASP.NET Core 9 (Clean Architecture)
│   ├── CookHomie.Domain/           # Entities, enums, interfaces
│   ├── CookHomie.Application/      # Use cases, DTOs, interfaces
│   ├── CookHomie.Infrastructure/   # EF Core, repositories, migrations
│   └── CookHomie.WebApi/           # Controllers, DI wiring
│
├── CookHomie.Web/                  # Nuxt 3 frontend
│   ├── pages/                      # UI pages
│   ├── components/                 # Vue components
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
- **Stack**: Nuxt 3 + Vue 3 | ASP.NET Core 9 + EF Core | PostgreSQL | Docker Compose

## Key Design Decisions
- **All IDs are UUIDs**: For future multi-user or sync scenarios
- **Tags as PostgreSQL arrays**: Avoid joins for MVP, filterable with `ANY()` queries
- **No auth for MVP**: Single user, local-only
- **Nuxt server/api proxy**: Avoids CORS in local development

## Documentation
Full architecture, data model, and roadmap documentation in the `docs/` directory:
- [[00-CookHomie]]
- [[01-Architecture]]
- [[02-DataModel]]
- [[03-RepoStructure]]
- [[04-Roadmap]]