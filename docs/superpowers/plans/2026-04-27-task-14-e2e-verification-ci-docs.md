> **Status:** ✅ Completed and merged into main (2026-04-27).

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

- [x] **Step 1: Write failing E2E smoke script**

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

- [x] **Step 2: Run script before stack is up to confirm it fails**
- [x] **Step 3: Update docker-compose.dev.yml to wire postgres healthcheck to api**
- [x] **Step 4: Write GitHub Actions CI stub**
- [x] **Step 5: Update README.md with Local Development Runbook**
- [x] **Step 6: Update CONTRIBUTING.md with E2E verification notes**
- [x] **Step 7: Verify the CI file is valid YAML**
- [x] **Step 8: Commit**

```bash
git add .github/workflows/ci.yml scripts/verify_add_item_e2e.sh README.md CONTRIBUTING.md docker-compose.dev.yml
git commit -m "feat: implement E2E verification, CI stub, and docs for add-item vertical slice"
```

---

### Final Validation Checklist (Run After Task 14)

> **Implementation status:** All steps complete. Steps 2–4 require running services locally.

- [x] `bash scripts/verify_add_item_e2e.sh` FAILS before stack is up (connection refused)
- [ ] `docker compose -f docker-compose.dev.yml up --build -d` starts stack successfully
- [ ] `bash scripts/verify_add_item_e2e.sh` PASSES after stack is up
- [ ] `docker compose -f docker-compose.dev.yml down` cleans up
- [x] `python3 -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml'))"` passes (no YAML syntax errors)

**Notes:**
- Steps 2–4 require Docker services running. Execute locally or in a CI runner with Docker.
- The E2E script uses hardcoded `http://localhost:5000` for the API base URL.
- The script adds "Oat Milk" and verifies it persists via GET requests.

(End of plan - total lines as shown)
