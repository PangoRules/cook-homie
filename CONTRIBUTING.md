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

## Spike verification command (Task 3)

- Use `dotnet test src/CookHomie.Api/CookHomie.SpikeApi.Tests/CookHomie.SpikeApi.Tests.csproj -f net10.0` for this environment's Task 3 spike verification.
- Note: this `-f net10.0` command is a spike-environment workaround for local runtime availability.
