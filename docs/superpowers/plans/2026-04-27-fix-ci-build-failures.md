# CI Build Failures — Fix Plan

> **For agentic workers:** Implementation instructions for the three CI failures.

---

## Issue 1: API — WebApi.Tests.Location Header Null

**Failure:** `Assert.NotNull(postResponse.Headers.Location)` fails at `InventoryControllerTests.cs:101`

**Root cause:** The `Created()` response in `InventoryController` sets a `Location` header to `"/api/inventory/{item.Id}"`, but the test asserts the URL must contain "api/inventory". The issue is the Location header is `null` entirely — meaning the controller is either not setting it, or the item.Id being returned is causing an issue.

**Fix:** Read `src/CookHomie.Api/CookHomie.WebApi/Controllers/InventoryController.cs` around the POST endpoint. Verify the `Created()` call passes a valid URI. The item.Id should be a valid GUID — check that `AddInventoryItemUseCase` returns an `InventoryItemDto` with the Id populated, and that the response type `InventoryItemResponse` exposes the Id correctly.

**Files to check:**
- `src/CookHomie.Api/CookHomie.WebApi/Controllers/InventoryController.cs` — POST method
- `src/CookHomie.Api/CookHomie.Application/UseCases/Inventory/AddInventoryItemUseCase.cs` — returns InventoryItemDto
- `src/CookHomie.Api/CookHomie.WebApi/Controllers/InventoryControllerTests.cs` — test at line 97-103

**Verify:** After fix, `postResponse.Headers.Location` should be non-null and contain "api/inventory".

---

## Issue 2: Web — `npm run lint` Missing Script

**Failure:** `npm error Missing script: "lint"`

**Root cause:** `package.json` has no `lint` script defined. Nuxt projects typically don't include ESLint by default.

**Fix:** Either remove the lint step from CI (if not needed), or add ESLint to the project. Since the MVP doesn't have a linting setup, the safer fix is to remove the lint step from `.github/workflows/ci.yml`:

```yaml
# Remove or comment out this step:
- name: Run Web lint
  run: npm run lint --prefix src/CookHomie.Web
```

**Alternative (if linting is desired):** Add ESLint with `npm install -D eslint @nuxt/eslint` and configure it, but that's overengineering for MVP.

**Recommended fix:** Remove the lint step from CI.

---

## Issue 3: MCP — `pytest` Module Not Found

**Failure:** `No module named pytest`

**Root cause:** `pip install -e src/CookHomie.MCP` installs the package but NOT the dev dependencies. `pytest` is listed under `[project.optional-dependencies] dev` in `pyproject.toml`, so it's not included in the base install.

**Fix:** Change the pip install command in CI to include the dev extras:

```yaml
- name: Install MCP dependencies
  run: pip install -e "src/CookHomie.MCP[dev]"
```

**Files to check:**
- `src/CookHomie.MCP/pyproject.toml` — line 16-18 confirms pytest is in dev dependencies

---

## Summary of Changes

| Job | File | Change |
|-----|------|--------|
| api-checks | `.github/workflows/ci.yml` | No change — API code needs fixing |
| web-checks | `.github/workflows/ci.yml` | Remove `npm run lint` step |
| mcp-checks | `.github/workflows/ci.yml` | Change `pip install -e src/CookHomie.MCP` to `pip install -e "src/CookHomie.MCP[dev]"` |

---

## Implementation Order

1. Fix `InventoryController.cs` or test — ensure Location header is set
2. Remove lint step from CI YAML
3. Fix pytest install command in CI YAML
4. Push fix commit, verify CI passes
