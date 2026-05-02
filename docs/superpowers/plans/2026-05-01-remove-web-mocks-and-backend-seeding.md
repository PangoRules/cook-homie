# Remove Web Mocks and Backend Seeding Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Remove Nuxt server mock data wherever C# API endpoints exist, keep only explicit dev-only or endpoint-missing mocks, and verify backend DB seeding owns test/demo data.

**Architecture:** Nuxt `server/api/**` routes must be thin proxy routes to C# API when backend endpoint exists. Backend remains source of truth for persistence and seed/demo data. Mock/in-memory server helpers must be deleted once replaced by real API proxy routes.

**Tech Stack:** Nuxt 3 server routes, Vitest, ASP.NET Core 10 WebApi, EF Core, PostgreSQL.

---

## Current Audit

### Nuxt server routes

| File | Current behavior | Backend endpoint exists? | Action |
|---|---|---:|---|
| `src/CookHomie.Web/server/api/inventory/index.get.ts` | Proxies to `/api/inventory` | Yes | Keep |
| `src/CookHomie.Web/server/api/inventory/index.post.ts` | Proxies to `/api/inventory` | Yes | Keep |
| `src/CookHomie.Web/server/api/hello.get.ts` | Proxies to C# API, but hardcodes `http://api:5000` | Yes | Normalize to `config.apiBaseUrl` |
| `src/CookHomie.Web/server/api/dashboard/summary.get.ts` | Hardcoded mock counts | No C# dashboard controller found | Keep for now, mark explicitly as temporary mock |
| `src/CookHomie.Web/server/api/recipes/index.get.ts` | Uses in-memory `recipeStore.ts` | No C# recipes controller found | Keep for now, mark explicitly as temporary mock |
| `src/CookHomie.Web/server/api/recipes/index.post.ts` | Uses in-memory `recipeStore.ts` | No C# recipes controller found | Keep for now, mark explicitly as temporary mock |
| `src/CookHomie.Web/server/api/recipes/[id].get.ts` | Uses in-memory `recipeStore.ts` | No C# recipes controller found | Keep for now, mark explicitly as temporary mock |
| `src/CookHomie.Web/server/api/recipes/[id]/missing.get.ts` | Uses in-memory `recipeStore.ts` | No C# recipes/missing endpoint found | Keep for now, mark explicitly as temporary mock |
| `src/CookHomie.Web/server/api/dev/inventory-seed.post.ts` | Pretends to seed inventory, does not touch DB | Backend already seeds inventory | Delete or replace with real backend dev endpoint later; for this plan delete because it lies |
| `src/CookHomie.Web/server/api/dev/mode.get.ts` | Returns env metadata | N/A | Keep |

### Backend endpoints found

- `GET /api/inventory` exists in `src/CookHomie.Api/CookHomie.WebApi/Controllers/InventoryController.cs`.
- `POST /api/inventory` exists in `src/CookHomie.Api/CookHomie.WebApi/Controllers/InventoryController.cs`.
- `GET /api/hello` exists in `src/CookHomie.Api/CookHomie.WebApi/Controllers/HelloController.cs`.
- No Recipes controller found.
- No Dashboard controller found.
- No Shopping controller found.

### Backend seeding found

- `src/CookHomie.Api/CookHomie.WebApi/Program.cs` runtime-seeds 8 inventory items when inventory table empty.
- `src/CookHomie.Api/CookHomie.Infrastructure/Persistence/AppDbContext.cs` also has `InventoryItem.HasData(...)` for same 8 inventory items.
- This is duplicate seeding. Keep one source of seed truth. Recommended follow-up: keep EF `HasData` for migration-managed baseline and remove runtime seed block from `Program.cs`.
- Recipes, shopping items, and dashboard demo values are not seeded because backend endpoints do not exist yet.

---

## Task 1 — Normalize existing real proxy routes

**Files:**
- Modify: `src/CookHomie.Web/server/api/hello.get.ts`
- Test: `src/CookHomie.Web/tests/hello-proxy.spec.ts`

- [ ] **Step 1: Write proxy test for hello route**

Create `src/CookHomie.Web/tests/hello-proxy.spec.ts`:

```ts
import { describe, expect, it, vi } from "vitest";

describe("hello proxy route", () => {
  it("GET proxy forwards to /api/hello on upstream API", async () => {
    const fetchSpy = vi.fn().mockResolvedValue({ message: "hello" });
    vi.stubGlobal("$fetch", fetchSpy);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api.example" }));

    const { default: handler } = await import("../server/api/hello.get");
    const result = await handler({} as never);

    expect(result).toEqual({ message: "hello" });
    expect(fetchSpy).toHaveBeenCalledWith("/api/hello", {
      baseURL: "http://api.example",
      method: "GET",
      headers: {
        Accept: "application/json",
      },
    });
  });
});
```

- [ ] **Step 2: Run test and verify failure**

Run:

```bash
cd src/CookHomie.Web
npm run test -- tests/hello-proxy.spec.ts
```

Expected: FAIL because current route hardcodes `http://api:5000/api/hello` instead of using `baseURL: config.apiBaseUrl`.

- [ ] **Step 3: Update hello proxy to use runtime config**

Replace `src/CookHomie.Web/server/api/hello.get.ts` with:

```ts
export default defineEventHandler(async (_event) => {
  const config = useRuntimeConfig();

  return await $fetch("/api/hello", {
    baseURL: config.apiBaseUrl,
    method: "GET",
    headers: {
      Accept: "application/json",
    },
  });
});
```

- [ ] **Step 4: Run test and verify pass**

Run:

```bash
cd src/CookHomie.Web
npm run test -- tests/hello-proxy.spec.ts
```

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/server/api/hello.get.ts src/CookHomie.Web/tests/hello-proxy.spec.ts
git commit -m "fix(web): normalize hello API proxy"
```

---

## Task 2 — Delete fake inventory seed Nuxt endpoint

**Files:**
- Delete: `src/CookHomie.Web/server/api/dev/inventory-seed.post.ts`
- Modify tests if any reference deleted route.

- [ ] **Step 1: Confirm no production code uses fake seed route**

Run:

```bash
cd /home/pango/Projects/cook-homie
rg "inventory-seed|/api/dev/inventory-seed" src/CookHomie.Web --glob '!\.nuxt/**' --glob '!\.output/**'
```

Expected: only the route file itself or generated Nuxt artifacts if not excluded. No app page/component/composable should depend on it.

- [ ] **Step 2: Delete fake route**

Run:

```bash
git rm src/CookHomie.Web/server/api/dev/inventory-seed.post.ts
```

- [ ] **Step 3: Run frontend tests**

Run:

```bash
cd src/CookHomie.Web
npm run test
```

Expected: PASS. If a test imported this route, delete that test or rewrite it to assert backend seeding instead.

- [ ] **Step 4: Commit**

```bash
git add -u src/CookHomie.Web/server/api/dev/inventory-seed.post.ts
git commit -m "chore(web): remove fake inventory seed route"
```

---

## Task 3 — Mark endpoint-missing mocks explicitly

**Files:**
- Modify: `src/CookHomie.Web/server/api/dashboard/summary.get.ts`
- Modify: `src/CookHomie.Web/server/utils/recipeStore.ts`
- Modify: `src/CookHomie.Web/server/api/recipes/index.get.ts`
- Modify: `src/CookHomie.Web/server/api/recipes/index.post.ts`
- Modify: `src/CookHomie.Web/server/api/recipes/[id].get.ts`
- Modify: `src/CookHomie.Web/server/api/recipes/[id]/missing.get.ts`

- [ ] **Step 1: Add explicit temporary mock comment to dashboard route**

Replace `src/CookHomie.Web/server/api/dashboard/summary.get.ts` with:

```ts
// TEMP_MOCK: No C# API endpoint exists yet for GET /api/dashboard/summary.
// Replace this route with a proxy when backend DashboardController exists.
export default defineEventHandler(() => ({
  expiringCount: 3,
  recipeMatchCount: 7,
  shoppingCount: 4,
}));
```

- [ ] **Step 2: Add explicit temporary mock comment to recipe store**

Insert this at top of `src/CookHomie.Web/server/utils/recipeStore.ts`:

```ts
// TEMP_MOCK: No C# Recipes API exists yet.
// This in-memory store must be deleted once backend RecipesController ships.
```

Keep existing imports and code after comment.

- [ ] **Step 3: Add explicit temporary mock comments to recipe routes**

For each recipe route file, add one first-line comment:

`src/CookHomie.Web/server/api/recipes/index.get.ts`:

```ts
// TEMP_MOCK: No C# API endpoint exists yet for GET /api/recipes.
import { getRecipes } from "~/server/utils/recipeStore";
export default defineEventHandler(() => getRecipes());
```

`src/CookHomie.Web/server/api/recipes/index.post.ts`:

```ts
// TEMP_MOCK: No C# API endpoint exists yet for POST /api/recipes.
import { addRecipe } from "~/server/utils/recipeStore";
export default defineEventHandler(async (event) => {
  const body = await readBody(event);
  if (!body?.name || !body?.instructions) {
    throw createError({ statusCode: 400, statusMessage: "name and instructions are required" });
  }
  return addRecipe(body);
});
```

`src/CookHomie.Web/server/api/recipes/[id].get.ts`:

```ts
// TEMP_MOCK: No C# API endpoint exists yet for GET /api/recipes/{id}.
import { getRecipeById } from "~/server/utils/recipeStore";
export default defineEventHandler((event) => {
  const id = getRouterParam(event, "id");
  const recipe = getRecipeById(id!);
  if (!recipe) {
    throw createError({ statusCode: 404, statusMessage: "Recipe not found" });
  }
  return recipe;
});
```

`src/CookHomie.Web/server/api/recipes/[id]/missing.get.ts`:

```ts
// TEMP_MOCK: No C# API endpoint exists yet for GET /api/recipes/{id}/missing.
import { getMissingIngredients } from "~/server/utils/recipeStore";
export default defineEventHandler((event) => {
  const id = getRouterParam(event, "id");
  return getMissingIngredients(id!, []);
});
```

- [ ] **Step 4: Run a mock audit command**

Run:

```bash
cd /home/pango/Projects/cook-homie
rg "TEMP_MOCK|recipeStore|expiringCount: 3|Classic Pancakes|new Random\(\)" src/CookHomie.Web/server src/CookHomie.Api --glob '!\.nuxt/**' --glob '!\.output/**'
```

Expected:
- `TEMP_MOCK` appears only in dashboard and recipe mock routes/store.
- `recipeStore` appears only in recipe mock routes/store/tests.
- `new Random()` appears only in `src/CookHomie.Api/Controllers/PollingController.cs`; this controller is not used by Nuxt server routes and should be separately reviewed or deleted if obsolete.

- [ ] **Step 5: Run frontend tests**

Run:

```bash
cd src/CookHomie.Web
npm run test
```

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/CookHomie.Web/server/api/dashboard/summary.get.ts src/CookHomie.Web/server/utils/recipeStore.ts src/CookHomie.Web/server/api/recipes/index.get.ts src/CookHomie.Web/server/api/recipes/index.post.ts src/CookHomie.Web/server/api/recipes/[id].get.ts src/CookHomie.Web/server/api/recipes/[id]/missing.get.ts
git commit -m "docs(web): label temporary server mocks"
```

---

## Task 4 — Make backend inventory seeding single-source

**Files:**
- Modify: `src/CookHomie.Api/CookHomie.WebApi/Program.cs`
- Test: `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/InventoryRepositoryTests.cs`
- Test: `src/CookHomie.Api/tests/CookHomie.WebApi.Tests/InventoryControllerTests.cs`

- [ ] **Step 1: Confirm EF HasData already seeds inventory**

Read `src/CookHomie.Api/CookHomie.Infrastructure/Persistence/AppDbContext.cs` and confirm `modelBuilder.Entity<InventoryItem>().HasData(...)` includes these IDs:

```text
d8109ce9-f967-4f32-a4a4-5031a0df1baf Milk
5a4b2996-ebf8-4b98-91fe-290ca2d9b2bd Cheddar Cheese
4f80dd83-a339-45bb-9f24-d76cc0f25818 Spinach
680aef2a-9969-4346-95d0-c6fc9f426445 Tomatoes
fd769e8e-252a-4e14-b0ec-87a5f4baa253 Chicken Breast
933d0ce1-faad-4a4b-b349-1d9f60dbb0be Canned Tuna
f6f06cae-72f7-4bc2-bf16-58b7d24924fe Rice
f92ff908-beb0-4af0-90d9-5992a39cc0bc Pasta
```

- [ ] **Step 2: Remove runtime duplicate seeding from Program.cs**

In `src/CookHomie.Api/CookHomie.WebApi/Program.cs`, delete lines equivalent to:

```csharp
    if (!dbContext.InventoryItems.Any())
    {
        var seededAt = DateTime.UtcNow;
        dbContext.InventoryItems.AddRange(
            new InventoryItem { Id = Guid.Parse("d8109ce9-f967-4f32-a4a4-5031a0df1baf"), Name = "Milk", Category = "Dairy", Location = Location.Fridge, Quantity = 1m, Unit = "liter", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("5a4b2996-ebf8-4b98-91fe-290ca2d9b2bd"), Name = "Cheddar Cheese", Category = "Dairy", Location = Location.Fridge, Quantity = 250m, Unit = "g", IsOpened = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("4f80dd83-a339-45bb-9f24-d76cc0f25818"), Name = "Spinach", Category = "Produce", Location = Location.Fridge, Quantity = 1m, Unit = "bag", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("680aef2a-9969-4346-95d0-c6fc9f426445"), Name = "Tomatoes", Category = "Produce", Location = Location.Fridge, Quantity = 6m, Unit = "units", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("fd769e8e-252a-4e14-b0ec-87a5f4baa253"), Name = "Chicken Breast", Category = "Protein", Location = Location.Freezer, Quantity = 2m, Unit = "units", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("933d0ce1-faad-4a4b-b349-1d9f60dbb0be"), Name = "Canned Tuna", Category = "Protein", Location = Location.Pantry, Quantity = 3m, Unit = "cans", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("f6f06cae-72f7-4bc2-bf16-58b7d24924fe"), Name = "Rice", Category = "Grains", Location = Location.Pantry, Quantity = 2m, Unit = "kg", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt },
            new InventoryItem { Id = Guid.Parse("f92ff908-beb0-4af0-90d9-5992a39cc0bc"), Name = "Pasta", Category = "Grains", Location = Location.Pantry, Quantity = 4m, Unit = "packs", IsOpened = false, CreatedAt = seededAt, UpdatedAt = seededAt });
        dbContext.SaveChanges();
    }
```

Also remove now-unused `using CookHomie.Domain.Entities;` and `using CookHomie.Domain.Enums;` from Program.cs.

- [ ] **Step 3: Run backend tests**

Run:

```bash
cd src/CookHomie.Api
dotnet test CookHomie.sln
```

Expected: PASS. Existing infrastructure test `EnsureCreated_SeedsTwoItemsPerCategory` should still pass because EF `HasData` remains.

- [ ] **Step 4: Run backend build**

Run:

```bash
cd src/CookHomie.Api
dotnet build CookHomie.sln
```

Expected: PASS with no unused using compile issues.

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Api/CookHomie.WebApi/Program.cs
git commit -m "chore(api): rely on EF inventory seed data"
```

---

## Task 5 — Add guardrail test: server mocks must be explicit

**Files:**
- Create: `src/CookHomie.Web/tests/server-mock-audit.spec.ts`

- [ ] **Step 1: Add audit test**

Create `src/CookHomie.Web/tests/server-mock-audit.spec.ts`:

```ts
import { describe, expect, it } from "vitest";
import { readFileSync, readdirSync, statSync } from "node:fs";
import { join } from "node:path";

const serverRoot = join(process.cwd(), "server");

const walk = (dir: string): string[] => readdirSync(dir).flatMap((entry) => {
  const path = join(dir, entry);
  return statSync(path).isDirectory() ? walk(path) : [path];
});

describe("server API mock audit", () => {
  it("requires obvious mock data in server routes to be marked TEMP_MOCK", () => {
    const offenders = walk(serverRoot)
      .filter(path => path.endsWith(".ts"))
      .filter((path) => {
        const source = readFileSync(path, "utf8");
        const looksMocked = [
          "Classic Pancakes",
          "Tomato Pasta",
          "Avocado Toast",
          "expiringCount: 3",
          "recipeMatchCount: 7",
          "shoppingCount: 4",
        ].some(marker => source.includes(marker));
        return looksMocked && !source.includes("TEMP_MOCK");
      });

    expect(offenders).toEqual([]);
  });
});
```

- [ ] **Step 2: Run audit test**

Run:

```bash
cd src/CookHomie.Web
npm run test -- tests/server-mock-audit.spec.ts
```

Expected: PASS after Task 3.

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/tests/server-mock-audit.spec.ts
git commit -m "test(web): guard explicit server mocks"
```

---

## Task 6 — Final validation

**Files:**
- No code changes expected.

- [ ] **Step 1: Run frontend validation**

Run:

```bash
cd src/CookHomie.Web
npm run validate
npm run test
```

Expected:
- `npm run validate` passes. Existing unrelated lint warning in `components/shared/TagInput.vue` must be fixed or acknowledged before PR depending repo policy.
- `npm run test` passes without Vue component warnings.

- [ ] **Step 2: Run backend validation**

Run:

```bash
cd src/CookHomie.Api
dotnet build CookHomie.sln
dotnet test CookHomie.sln
```

Expected: PASS.

- [ ] **Step 3: Run final mock audit**

Run:

```bash
cd /home/pango/Projects/cook-homie
rg "TEMP_MOCK|recipeStore|expiringCount: 3|Classic Pancakes|inventory-seed|new Random\(\)" src/CookHomie.Web/server src/CookHomie.Api --glob '!\.nuxt/**' --glob '!\.output/**'
```

Expected:
- `inventory-seed` absent from `src/CookHomie.Web/server`.
- `TEMP_MOCK` present only where backend endpoint missing.
- `recipeStore` only exists because recipes API missing.
- `new Random()` only in unused `PollingController.cs`; open separate cleanup if no route consumes it.

---

## Follow-up Work Not In This Plan

1. Add C# Recipes API (`GET /api/recipes`, `POST /api/recipes`, `GET /api/recipes/{id}`, `GET /api/recipes/{id}/missing`) and then delete `recipeStore.ts` plus all recipe TEMP_MOCK routes.
2. Add C# Dashboard API (`GET /api/dashboard/summary`) and then replace hardcoded dashboard Nuxt route with proxy.
3. Add C# Shopping API and Nuxt proxy routes for `useShoppingList` (`/api/shopping`) because web composable references routes that do not exist yet.
4. Review/delete `src/CookHomie.Api/Controllers/PollingController.cs`; it returns random mock data and appears unrelated to current Nuxt app.
