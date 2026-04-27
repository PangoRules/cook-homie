# Task 15: Cut Over to WebApi and Retire Spike from Default Flow

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Verify the API Docker runtime points at `CookHomie.WebApi.dll`, make spike verification optional/non-blocking, and ensure dev compose properly delegates to WebApi.

**Architecture:** The API container already runs `CookHomie.WebApi` via `docker/Dockerfile.api`. Remaining work: (1) fix `docker-compose.dev.yml` to run WebApi with hot-reload instead of placeholder sleep, (2) add `ENABLE_SPIKE_CHECK` env-var guard to `verify_spike.sh`, (3) verify end-to-end stack and Swagger.

**Tech Stack:** Docker Compose, ASP.NET Core 10, Bash

---

## Pre-Flight: Verify Current State

Before starting, confirm the Dockerfile already points at WebApi:

- [ ] **Step 1: Verify Dockerfile.api references CookHomie.WebApi.dll**

Run: `grep -q 'CookHomie.WebApi.dll' docker/Dockerfile.api && echo "PASS: Dockerfile already targets WebApi" || echo "FAIL"`
Expected: `PASS: Dockerfile already targets WebApi`

---

## Task 15.1: Fix docker-compose.dev.yml to Run WebApi Properly

**Files:**
- Modify: `docker-compose.dev.yml:16-25`

The api service currently uses a placeholder command that sleeps forever instead of running WebApi. Update it to mount source and run the WebApi project with the ASP.NET dev server.

- [ ] **Step 1: Read current dev compose api service**

The current api service section (lines 16-25):
```yaml
  api:
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      CONNECTIONSTRINGS_DEFAULTCONNECTION: Host=postgres;Database=${POSTGRES_DB:-cookhomie};Username=${POSTGRES_USER:-cookhomie};Password=${POSTGRES_PASSWORD:-cookhomie_dev_password}
    volumes:
      - ./src/CookHomie.Api:/app
    command: ["sh", "-c", "echo 'API dev placeholder (hot reload pending scaffold)'; sleep infinity"]
    depends_on:
      postgres:
        condition: service_healthy
```

- [ ] **Step 2: Replace api service with proper WebApi dev runner**

```yaml
  api:
    build:
      context: .
      dockerfile: docker/Dockerfile.api
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:5000
      CONNECTIONSTRINGS_DEFAULTCONNECTION: Host=postgres;Database=${POSTGRES_DB:-cookhomie};Username=${POSTGRES_USER:-cookhomie};Password=${POSTGRES_PASSWORD:-cookhomie_dev_password}
    volumes:
      - ./src/CookHomie.Api:/src
    working_dir: /src/CookHomie.WebApi
    command: ["dotnet", "run", "--project", "CookHomie.WebApi.csproj"]
    ports:
      - "127.0.0.1:${API_PORT:-5000}:5000"
    depends_on:
      postgres:
        condition: service_healthy
```

- [ ] **Step 3: Validate compose syntax**

Run: `docker compose -f docker-compose.dev.yml config >/dev/null && echo "PASS: dev compose is valid" || echo "FAIL: compose config error"`
Expected: `PASS: dev compose is valid`

- [ ] **Step 4: Commit**

```bash
git add docker-compose.dev.yml
git commit -m "fix(dev): run CookHomie.WebApi in docker-compose.dev.yml instead of placeholder"
```

---

## Task 15.2: Make verify_spike.sh Optional and Non-Blocking

**Files:**
- Modify: `scripts/verify_spike.sh`

- [ ] **Step 1: Add ENABLE_SPIKE_CHECK guard at top of script**

Replace the curl checks at the bottom of the script with an early-exit guard. The script should:
1. Check if `ENABLE_SPIKE_CHECK` env var is set to `1`
2. If not, echo a skip message and exit `0`
3. If set to `1`, run the existing endpoint checks

```bash
#!/usr/bin/env bash

set -euo pipefail

if [[ "${ENABLE_SPIKE_CHECK:-0}" != "1" ]]; then
  echo "Skipping spike verification (set ENABLE_SPIKE_CHECK=1 to enable)."
  exit 0
fi

API_PORT="${API_PORT:-5000}"
WEB_PORT="${WEB_PORT:-3000}"

# ... rest of existing curl checks ...
```

- [ ] **Step 2: Verify the script still works with the flag off**

Run: `bash scripts/verify_spike.sh`
Expected output: `Skipping spike verification (set ENABLE_SPIKE_CHECK=1 to enable.)` and exit code `0`

- [ ] **Step 3: Commit**

```bash
git add scripts/verify_spike.sh
git commit -m "feat(spike): make verify_spike.sh optional via ENABLE_SPIKE_CHECK=1"
```

---

## Task 15.3: End-to-End Stack Verification

**Files:**
- None (verification only)

- [ ] **Step 1: Start the full stack with docker-compose.yml**

Run: `docker compose -f docker-compose.yml up --build -d`
Expected: All services start without error

- [ ] **Step 2: Wait for services to be healthy**

Run: `sleep 15 && curl -fsS http://localhost:5000/health`
Expected: `200 OK` with health details JSON

- [ ] **Step 3: Run E2E add-item verification**

Run: `bash scripts/verify_add_item_e2e.sh`
Expected: `E2E add-item verification passed`

- [ ] **Step 4: Verify Swagger is reachable**

Run: `curl -fsS http://localhost:5000/swagger/v1/swagger.json >/dev/null && echo "PASS: Swagger JSON reachable"`
Expected: `PASS: Swagger JSON reachable`

Run: `curl -fsSI http://localhost:5000/swagger | grep -E 'HTTP|200|301|302'`
Expected: `200 OK` or `301` redirect

- [ ] **Step 5: Tear down stack**

Run: `docker compose -f docker-compose.yml down`
Expected: Stack stops cleanly

---

## Task 15.4: Update README with Spike Status

**Files:**
- Modify: `README.md`

- [ ] **Step 1: Update spike section to reflect optional status**

Replace the "Spike validation (historical)" section (lines 26-32) with:

```markdown
## Spike Validation (Optional)

The spike API (`CookHomie.SpikeApi`) is historical validation code. It is not part of the default Docker runtime.

To run spike verification:
1. `export ENABLE_SPIKE_CHECK=1`
2. Run: `bash scripts/verify_spike.sh`
3. Unset when done: `unset ENABLE_SPIKE_CHECK`
```

- [ ] **Step 2: Commit**

```bash
git add README.md
git commit -m "docs: update README spike section to reflect optional status"
```

---

## Final Validation Checklist

After completing all tasks, run:

- [ ] `grep -q 'CookHomie.WebApi.dll' docker/Dockerfile.api && echo "PASS: Dockerfile targets WebApi"`
- [ ] `docker compose -f docker-compose.dev.yml config >/dev/null && echo "PASS: dev compose valid"`
- [ ] `bash scripts/verify_spike.sh` → expect skip message with exit 0
- [ ] `docker compose -f docker-compose.yml up --build -d`
- [ ] `curl -fsS http://localhost:5000/health` → expect 200
- [ ] `bash scripts/verify_add_item_e2e.sh` → expect E2E pass
- [ ] `curl -fsS http://localhost:5000/swagger/v1/swagger.json >/dev/null && echo "PASS: Swagger JSON"`
- [ ] `docker compose -f docker-compose.yml down`

Expected: All checks pass; spike verification is skipped by default.