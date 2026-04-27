# CookHomie

CookHomie is the food and kitchen module of HomieOS. It is a local-first app for pantry inventory, recipe suggestions, and shopping lists, with an MCP server for AI integrations.

## Monorepo services

- `CookHomie.Web` (Nuxt 3 frontend)
- `CookHomie.Api` (ASP.NET Core Web API)
- `CookHomie.MCP` (Python FastMCP server)

## Quick start (Docker Compose)

1. Copy env vars: `cp .env.example .env`
2. Start local stack: `docker compose up --build`
3. Open web app: `http://localhost:3000`

Notes:

- The `api` service exposes inventory endpoints under `http://localhost:5000/api/inventory`.
- `POST /api/inventory` validates required fields (`name`, `category`, `location`, `quantity > 0`, `unit`) and returns `201` with the created item.
- Invalid payloads return `400` with `application/problem+json`.
- Swagger UI is available in development at `http://localhost:5000/swagger`.

## Common commands

### .NET API

```bash
dotnet build src/CookHomie.Api/CookHomie.sln   # build
dotnet clean src/CookHomie.Api/CookHomie.sln   # clean
dotnet test  src/CookHomie.Api/CookHomie.sln   # run tests
```

### Frontend (Nuxt)

```bash
cd src/CookHomie.Web
npm run dev    # dev server (http://localhost:3000)
npm run build  # production build
npm test       # run tests
```

### MCP server

```bash
cd src/CookHomie.MCP
.venv/bin/python -m pytest -q  # run tests
```

### Docker

```bash
docker compose up -d                          # start all services (production-like)
docker compose -f docker-compose.dev.yml up   # start with live reload via volumes
docker compose down                           # stop all services
docker compose logs -f                        # stream logs
```

## Stack verification checklist

Run these from repo root to verify the scaffolded stack:

- API build: `dotnet build src/CookHomie.Api/CookHomie.sln`
- API tests: `dotnet test src/CookHomie.Api/CookHomie.sln`
- Manual inventory API scenarios: run requests in `src/CookHomie.Api/tests/ApiTests/inventory-controller-tests.http`
- Web tests/build/audit: `cd src/CookHomie.Web && npm test && npm run build && npm audit`
- MCP tests: `cd src/CookHomie.MCP && .venv/bin/python -m pytest -q`
- Default API runtime check: `docker compose up --build -d postgres api && curl -fsS http://localhost:5000/health`

## Runtime versions

- .NET projects under `src/CookHomie.Api` target `net10.0`.
- The API Docker image uses `.NET SDK 10.0` and `ASP.NET Runtime 10.0`.

## Default local ports

| Service | Port |
| --- | --- |
| Nuxt Web | `3000` |
| C# API | `5000` |
| MCP Server | `8000` |
| PostgreSQL | `5432` |

## Local Development Runbook

1. Copy env vars: `cp .env.example .env`
2. Start the stack with Docker Compose: `docker compose up --build`
3. Access the web app: `http://localhost:3000`
4. Verify the stack is healthy: `curl -fsS http://localhost:5000/health`
5. Run the E2E smoke check: `bash scripts/verify_add_item_e2e.sh`
6. Tear down the stack: `docker compose down`
