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

- The default `api` service runs `CookHomie.WebApi` and exposes inventory endpoints under `http://localhost:5000/api/inventory`.
- `POST /api/inventory` routes creation through the application use case and validates required fields (`name`, `category`, `location`, `quantity > 0`, `unit`).
- Invalid `POST /api/inventory` payloads return `400` with `application/problem+json`.
- Current create response behavior is `201` with the created item body; clients should not rely on a `Location` header yet.
- Swagger is available in development at `http://localhost:5000/swagger`.
- The spike API still exists in source for historical validation, but it is not the default Docker runtime.

## Spike Validation (Optional)

The spike API (`CookHomie.SpikeApi`) is historical validation code. It is not part of the default Docker runtime.

To run spike verification:
1. `export ENABLE_SPIKE_CHECK=1`
2. Run: `bash scripts/verify_spike.sh`
3. Unset when done: `unset ENABLE_SPIKE_CHECK`

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
