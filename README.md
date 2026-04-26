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

- The `api` service currently runs `CookHomie.SpikeApi` and exposes `http://localhost:5000/spike/hello`.
- The `mcp` Docker service is still a placeholder container. Run MCP locally from `src/CookHomie.MCP` when validating MCP behavior.

## Spike validation

Use the end-to-end spike checker to validate both direct API reachability and the Nuxt proxy:

1. Run before services are up (expected to fail): `./scripts/verify_spike.sh`
2. Start services needed for spike validation (avoids MCP port conflicts): `docker compose up --build -d postgres api web`
3. Run again (expected to pass): `./scripts/verify_spike.sh`

## Stack verification checklist

Run these from repo root to verify the scaffolded stack:

- API build: `dotnet build src/CookHomie.Api/CookHomie.sln`
- API tests: `dotnet test src/CookHomie.Api/CookHomie.sln`
- Web tests/build/audit: `cd src/CookHomie.Web && npm test && npm run build && npm audit`
- MCP tests: `cd src/CookHomie.MCP && .venv/bin/python -m pytest -q`

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
