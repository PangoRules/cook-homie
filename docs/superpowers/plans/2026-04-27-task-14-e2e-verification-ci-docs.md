# Task 14: End-to-End Verification + CI Stub + Docs Sync

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add GitHub Actions CI stub, create an E2E smoke script for the add-item vertical slice, and synchronize README/CONTRIBUTING with the current dev runbook.

**Architecture:** E2E smoke script hits API + Web endpoints directly via curl. CI stub runs build/test/lint for all three services in parallel on Ubuntu. Docs updated to reflect current compose file naming (`docker-compose.dev.yml`).

**Tech Stack:** GitHub Actions, Docker Compose, curl

---

### Task 14: End-to-End Verification + CI Stub + Docs Sync

**Files:**
- Create: `.github/workflows/ci.yml`
- Create: `scripts/verify_add_item_e2e.sh`
- Modify: `README.md` (add Local Development Runbook section)
- Modify: `CONTRIBUTING.md` (add E2E verification notes)
- Modify: `docker-compose.dev.yml` (add postgres healthcheck dependency for API)

- [ ] **Step 1: Write failing E2E smoke script**

Create `scripts/verify_add_item_e2e.sh`:

```bash
#!/usr/bin/env bash
set -euo pipefail

API_PORT="${API_PORT:-5000}"
WEB_PORT="${WEB_PORT:-3000}"

if ! command -v curl >/dev/null 2>&1; then
  printf '[FAIL] curl is required for E2E verification. Install curl and retry.\n'
  exit 2
fi

# Add an inventory item via the API
add_response="$(curl --silent --show-error --fail --max-time 10 \
  -X POST "http://localhost:${API_PORT}/api/inventory" \
  -H 'Content-Type: application/json' \
  -d '{"name":"Oat Milk","category":"dairy","location":"Fridge","quantity":1.0,"unit":"liter"}')"

if ! echo "$add_response" | grep -q '"name"'; then
  printf '[FAIL] API POST /api/inventory did not return expected JSON.\n'
  exit 1
fi

# Verify it appears in the API list
list_response="$(curl --silent --show-error --fail --max-time 10 \
  "http://localhost:${API_PORT}/api/inventory")"

if ! echo "$list_response" | grep -q "Oat Milk"; then
  printf '[FAIL] API GET /api/inventory does not contain added item.\n'
  exit 1
fi

# Verify it appears via the web proxy
web_list_response="$(curl --silent --show-error --fail --max-time 10 \
  "http://localhost:${WEB_PORT}/api/inventory")"

if ! echo "$web_list_response" | grep -q "Oat Milk"; then
  printf '[FAIL] Web GET /api/inventory does not contain added item.\n'
  exit 1
fi

echo "[PASS] E2E add-item verification passed"
```

- [ ] **Step 2: Run script before stack is up to confirm it fails**

Run: `bash scripts/verify_add_item_e2e.sh`
Expected: FAIL with connection refused (services not running)

- [ ] **Step 3: Update docker-compose.dev.yml to wire postgres healthcheck to api**

```yaml
services:
  postgres:
    # ... existing postgres config ...
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-cookhomie} -d ${POSTGRES_DB:-cookhomie}"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    # ... existing api config ...
    depends_on:
      postgres:
        condition: service_healthy
```

- [ ] **Step 4: Write GitHub Actions CI stub**

Create `.github/workflows/ci.yml`:

```yaml
name: CI

on:
  push:
    branches: [main, feat/*]
  pull_request:

jobs:
  api-checks:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore API packages
        run: dotnet restore src/CookHomie.Api/CookHomie.sln

      - name: Build API solution
        run: dotnet build src/CookHomie.Api/CookHomie.sln --no-restore

      - name: Run API tests
        run: dotnet test src/CookHomie.Api/CookHomie.sln --no-build --verbosity quiet

  web-checks:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: src/CookHomie.Web/package-lock.json

      - name: Install Web dependencies
        run: cd src/CookHomie.Web && npm ci

      - name: Run Web tests
        run: cd src/CookHomie.Web && npm test

  mcp-checks:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-python@v5
        with:
          python-version: '3.11'

      - name: Install MCP dependencies
        run: cd src/CookHomie.MCP && pip install -e .

      - name: Run MCP tests
        run: cd src/CookHomie.MCP && pytest -q
```

- [ ] **Step 5: Update README.md with Local Development Runbook**

Add to end of README.md:

```markdown
## Local Development Runbook

1. Copy env vars: `cp .env.example .env`
2. Start the full stack: `docker compose -f docker-compose.dev.yml up --build`
3. Open web app: `http://localhost:3000`
4. Verify the stack is healthy:
   - `curl -fsS http://localhost:5000/health` (API + DB health check)
   - `curl -fsS http://localhost:5000/swagger/v1/swagger.json` (OpenAPI spec reachable)
5. Run E2E add-item smoke check:
   - `bash scripts/verify_add_item_e2e.sh`
6. Tear down: `docker compose -f docker-compose.dev.yml down`
```

- [ ] **Step 6: Update CONTRIBUTING.md with E2E verification notes**

Add after the existing MCP local test workflow section:

```markdown
## E2E verification workflow

The E2E smoke script verifies the add-item vertical slice end-to-end.

1. Start the dev stack: `docker compose -f docker-compose.dev.yml up --build`
2. Run the E2E smoke check:
   ```bash
   bash scripts/verify_add_item_e2e.sh
   ```
3. Expected output: `[PASS] E2E add-item verification passed`

Notes:
- The script requires `curl` and the stack running on default ports.
- It adds a temporary item ("Oat Milk") and verifies it appears in API and Web proxy responses.
- Tear down with `docker compose -f docker-compose.dev.yml down`.
```

- [ ] **Step 7: Verify the CI file is valid YAML**

Run: `docker compose -f .github/workflows/ci.yml config 2>&1 || true` (YAML validation via compose tool — will fail but confirms file is readable)
Alternative: `python3 -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml'))"`

Expected: no syntax errors

- [ ] **Step 8: Commit**

```bash
git add .github/workflows/ci.yml scripts/verify_add_item_e2e.sh README.md CONTRIBUTING.md docker-compose.dev.yml
git commit -m "chore: add e2e verification script ci stub and dev runbook"
```

---

### Final Validation Checklist (Run After Task 14)

- [ ] `bash scripts/verify_add_item_e2e.sh` FAILS before stack is up (connection refused)
- [ ] `docker compose -f docker-compose.dev.yml up --build -d` starts stack successfully
- [ ] `bash scripts/verify_add_item_e2e.sh` PASSES after stack is up
- [ ] `docker compose -f docker-compose.dev.yml down` cleans up
- [ ] `python3 -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml'))"` passes (no YAML syntax errors)

(End of plan - total lines as shown)
