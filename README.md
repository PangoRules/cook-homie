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

## Spike validation (historical)

Use the spike checker only when explicitly running `CookHomie.SpikeApi` locally. The default Docker stack runs `CookHomie.WebApi`, so this check is not part of the normal validation path.

1. Run before services are up (expected to fail): `./scripts/verify_spike.sh`
2. Start a local spike API and Nuxt web process that proxies `/api/spike/hello`.
3. Run again (expected to pass): `./scripts/verify_spike.sh`

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
