# Contributing

Thanks for helping build CookHomie.

## Local setup

1. Copy env vars: `cp .env.example .env`
2. Start dependencies and services: `docker compose up --build`
3. Work inside this monorepo and keep changes scoped to one task per commit.

## Basic workflow

1. Create a branch for your task.
2. Make small, focused changes.
3. Run relevant checks locally before committing.
4. Use clear commit messages describing intent.
5. Open a PR with a short summary and verification notes.

## API local test workflow

- Use `dotnet test src/CookHomie.Api/CookHomie.SpikeApi.Tests/CookHomie.SpikeApi.Tests.csproj -f net10.0` for spike endpoint verification.
- The API projects target .NET 10 (`net10.0`) across the solution.
- Use `dotnet test src/CookHomie.Api/CookHomie.sln` for the normal API verification path.
- For manual endpoint checks, run the HTTP scenarios in `src/CookHomie.Api/tests/ApiTests/inventory-controller-tests.http`.
- Current inventory controller manual checks should cover:
  - `GET /api/inventory` returns `200`
  - `POST /api/inventory` valid payload returns `201`
  - `POST /api/inventory` invalid payload returns `400` Problem Details
- The default Docker API runtime is `CookHomie.WebApi`; `CookHomie.SpikeApi` remains available only for historical spike checks.

## MCP local test workflow

- Run MCP tests from `src/CookHomie.MCP` using the project virtual environment, not system Python.
- From repo root, run: `cd src/CookHomie.MCP && .venv/bin/python -m pytest -q`.

## Web local test workflow

- From repo root, run: `cd src/CookHomie.Web && npm test && npm run build && npm audit`.
