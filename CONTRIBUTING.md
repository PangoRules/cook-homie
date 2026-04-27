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

- The API projects target .NET 10 (`net10.0`) across the solution.
- Use `dotnet test src/CookHomie.Api/CookHomie.sln` for the normal API verification path.
- For manual endpoint checks, run the HTTP scenarios in `src/CookHomie.Api/tests/ApiTests/inventory-controller-tests.http`.
- Current inventory controller manual checks should cover:
  - `GET /api/inventory` returns `200`
  - `POST /api/inventory` valid payload returns `201`
  - `POST /api/inventory` invalid payload returns `400` Problem Details

## MCP local test workflow

- Run MCP tests from `src/CookHomie.MCP` using the project virtual environment, not system Python.
- From repo root, run: `cd src/CookHomie.MCP && .venv/bin/python -m pytest -q`.

## E2E verification workflow

The E2E smoke script verifies the add-item vertical slice end-to-end.

1. Start the dev stack: `docker compose up --build`
2. Run the E2E smoke check:
   ```bash
   bash scripts/verify_add_item_e2e.sh
   ```
3. Expected output: `🎉 All tests passed! The add-item vertical slice is working correctly.`

Notes:
- The script requires `curl` and the stack running on default ports.
- It adds a temporary item ("Oat Milk") and verifies it appears in API and Web proxy responses.
- Tear down with `docker compose down`.

## Web local test workflow

- From repo root, run: `cd src/CookHomie.Web && npm test && npm run build && npm audit`.
