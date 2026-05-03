# Milestone 2 Frontend Gaps — Corrective Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix the 4 remaining gaps found in the code review: (1) dashboard panel data types/shape, (2) `useRecipes`/`useInventory` raw timer ownership, (3) `recipeStore.addRecipe` ingredient persistence, (4) `AddRecipePayload` type with ingredients field.

**Architecture:** Align the mock data contracts first (types + mock endpoint), then update pages to consume the correct shapes, then fix composable timer delegation, then fix store persistence. Each task is self-contained and produces a commit-able unit.

**Tech Stack:** Nuxt 3, Vue 3 Composition API, TypeScript, Vitest, Vue Test Utils, Tailwind v4 CSS-first.

---

## Before Starting — Read Current Files

The engineer MUST read these files before touching anything:

```
src/CookHomie.Web/types/index.ts                           — current shared types
src/CookHomie.Web/server/api/dashboard/summary.get.ts    — current mock endpoint
src/CookHomie.Web/server/utils/recipeStore.ts            — current mock store
src/CookHomie.Web/composables/useRecipes.ts               — timer owner to refactor
src/CookHomie.Web/composables/useInventory.ts             — timer owner to refactor
src/CookHomie.Web/composables/usePollingFetch.ts          — visibility-aware base to reuse
src/CookHomie.Web/pages/index.vue                         — dashboard page to update
src/CookHomie.Web/components/recipes/AddRecipeModal.vue  — modal to verify
src/CookHomie.Web/tests/recipe-types.spec.ts              — add-type tests here
src/CookHomie.Web/tests/recipe-store.spec.ts              — add-persistence test here
src/CookHomie.Web/tests/usePollingFetch.spec.ts           — visibility test already exists
```

---

## Task 1: Align Dashboard Data Contracts (Types + Mock + Page)

### Problem
`DashboardSummary` uses `upcomingExpirations: string[]` and `recommendedRecipes: string[]` but the spec and page expect structured arrays with `expiringItems: DashboardExpiringItem[]` and `recipeIdeas: DashboardRecipeIdea[]` containing `name`, `location`, `matchedCount`, `missingCount`. The page (`pages/index.vue`) currently renders only date strings, not item details.

### Files
- Modify: `src/CookHomie.Web/types/index.ts`
- Modify: `src/CookHomie.Web/server/api/dashboard/summary.get.ts`
- Modify: `src/CookHomie.Web/pages/index.vue`
- Test: `src/CookHomie.Web/tests/recipe-types.spec.ts`

### Steps

- [ ] **Step 1: Write failing type + render tests**

In `src/CookHomie.Web/tests/recipe-types.spec.ts`, add:

```ts
import type { DashboardSummary, AddRecipePayload, RecipeIngredient } from "../types";

it("has dashboard summary with structured expiringItems", () => {
  const summary: DashboardSummary = {
    expiringCount: 3,
    recipeMatchCount: 2,
    shoppingCount: 4,
    totalItems: 15,
    expiringItems: [
      { id: "inv-1", name: "Milk", expiresAt: "2026-05-03", location: "Fridge" },
    ],
    recipeIdeas: [
      { id: "r1", name: "Classic Pancakes", matchedCount: 2, missingCount: 1 },
    ],
  };
  expect(summary.expiringItems[0].name).toBe("Milk");
  expect(summary.expiringItems[0].location).toBe("Fridge");
  expect(summary.recipeIdeas[0].matchedCount).toBe(2);
  expect(summary.recipeIdeas[0].missingCount).toBe(1);
});

it("has add recipe payload that includes ingredients", () => {
  const payload: AddRecipePayload = {
    name: "Toast",
    instructions: "Toast bread.",
    prepMinutes: 1,
    cookMinutes: 2,
    tags: ["breakfast"],
    ingredients: [
      { ingredientName: "Bread", quantity: 2, unit: "slices", isOptional: false },
    ],
  };
  expect(payload.ingredients[0].ingredientName).toBe("Bread");
});
```

- [ ] **Step 2: Run test to verify failure**

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-types.spec.ts
```
Expected: FAIL — `expiringItems`, `recipeIdeas`, and `ingredients` not assignable on current types.

- [ ] **Step 3: Update shared types**

Replace the `DashboardSummary` block in `src/CookHomie.Web/types/index.ts`:

```ts
export interface DashboardExpiringItem {
  id: string;
  name: string;
  expiresAt: string;
  location: string;
}

export interface DashboardRecipeIdea {
  id: string;
  name: string;
  matchedCount: number;
  missingCount: number;
}

export interface DashboardSummary {
  expiringCount: number;
  recipeMatchCount: number;
  shoppingCount: number;
  totalItems: number;
  expiringItems: DashboardExpiringItem[];
  recipeIdeas: DashboardRecipeIdea[];
}

export type AddRecipeIngredientPayload = Omit<RecipeIngredient, "id" | "recipeId" | "isInStock">;

export type AddRecipePayload = Omit<Recipe, "id" | "ingredients"> & {
  ingredients: AddRecipeIngredientPayload[];
};
```

- [ ] **Step 4: Update mock endpoint**

Replace `src/CookHomie.Web/server/api/dashboard/summary.get.ts` with:

```ts
// TEMP_MOCK: No C# API endpoint exists yet for GET /api/dashboard/summary.
import type { DashboardSummary } from "~/types";

export default defineEventHandler((): DashboardSummary => ({
  expiringCount: 3,
  recipeMatchCount: 2,
  shoppingCount: 4,
  totalItems: 15,
  expiringItems: [
    { id: "inv-1", name: "Milk",     expiresAt: "2026-05-03", location: "Fridge"   },
    { id: "inv-2", name: "Spinach",  expiresAt: "2026-05-04", location: "Fridge"   },
    { id: "inv-3", name: "Yogurt",   expiresAt: "2026-05-05", location: "Fridge"   },
  ],
  recipeIdeas: [
    { id: "r1", name: "Classic Pancakes", matchedCount: 2, missingCount: 1 },
    { id: "r3", name: "Avocado Toast",    matchedCount: 2, missingCount: 0 },
  ],
}));
```

- [ ] **Step 5: Update dashboard page panel content**

In `src/CookHomie.Web/pages/index.vue`, replace the first panel slot (inside `<DashboardPanel title="Expiring Soon">`) with:

```vue
<template #default>
  <ul v-if="data?.expiringItems?.length" class="list-none p-0 m-0 flex flex-col gap-3">
    <li
      v-for="item in data.expiringItems"
      :key="item.id"
      class="flex items-center justify-between gap-3 rounded-md border border-border bg-bg p-3"
    >
      <div class="flex flex-col gap-0.5">
        <span class="font-semibold text-text-primary">{{ item.name }}</span>
        <span class="text-xs text-text-muted">{{ item.location }}</span>
      </div>
      <span class="text-xs font-semibold text-warning">{{ formatExpiry(item.expiresAt) }}</span>
    </li>
  </ul>
  <p v-else-if="!loading" class="text-text-muted text-sm italic">
    No expiring items — inventory looks fresh!
  </p>
</template>
```

Replace the second panel slot (inside `<DashboardPanel title="Quick Recipe Ideas">`) with:

```vue
<template #default>
  <ul v-if="data?.recipeIdeas?.length" class="list-none p-0 m-0 flex flex-col gap-3">
    <li
      v-for="recipe in data.recipeIdeas"
      :key="recipe.id"
      class="rounded-md border border-border bg-bg p-3"
    >
      <NuxtLink
        :to="`/recipes/${recipe.id}`"
        class="font-semibold text-text-primary no-underline"
      >
        {{ recipe.name }}
      </NuxtLink>
      <p class="m-0 mt-1 text-xs text-text-muted">
        {{ recipe.matchedCount }} in stock · {{ recipe.missingCount }} missing
      </p>
    </li>
  </ul>
  <p v-else-if="!loading" class="text-text-muted text-sm italic">
    Add inventory items to get recipe suggestions.
  </p>
</template>
```

Keep the existing `formatExpiry` helper function — it formats a date string. The `expiringItems[].expiresAt` field is already a date string, so `formatExpiry(item.expiresAt)` works as-is.

- [ ] **Step 6: Run focused tests**

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-types.spec.ts tests/index.spec.ts tests/dashboard.spec.ts
```
Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add src/CookHomie.Web/types/index.ts src/CookHomie.Web/server/api/dashboard/summary.get.ts src/CookHomie.Web/pages/index.vue src/CookHomie.Web/tests/recipe-types.spec.ts
git commit -m "fix: align dashboard data contracts with spec"
```

---

## Task 2: Refactor useRecipes to Use usePollingFetch

### Problem
`useRecipes.ts` owns a raw `setInterval` instead of delegating to `usePollingFetch`. The same applies to `useInventory.ts` and `useShoppingList.ts`. This means they don't pause when the page is hidden.

### Files
- Modify: `src/CookHomie.Web/composables/useRecipes.ts`
- Modify: `src/CookHomie.Web/composables/useInventory.ts`
- Modify: `src/CookHomie.Web/composables/useShoppingList.ts`
- Test: `src/CookHomie.Web/tests/recipes-list.spec.ts`, `src/CookHomie.Web/tests/useInventory.spec.ts`

### Design Decision
`useRecipes` should delegate entirely to `usePollingFetch` — replace `recipes` state with a computed from `polling.data`. For `useInventory` and `useShoppingList`, keep their mutation methods (`addInventoryItem`, `addToList`) but replace timer ownership with `usePollingFetch`. Preserve returned keys: `startPolling`, `stopPolling`, `refresh`, `loadInventory`, `loadList`.

### Steps

- [ ] **Step 1: Write failing recipes list test**

Verify the test in `src/CookHomie.Web/tests/recipes-list.spec.ts` calls the polling composable. The existing test is sufficient but verify it still passes after the refactor — add a visibility check if not present.

Run first to confirm current state:
```bash
cd src/CookHomie.Web
npm test -- tests/recipes-list.spec.ts
```
Expected: PASS (baseline before refactor).

- [ ] **Step 2: Refactor useRecipes**

Replace `src/CookHomie.Web/composables/useRecipes.ts` content with:

```ts
import type { Recipe } from "~/types";

export const useRecipes = () => {
  const polling = usePollingFetch<Recipe[]>("/api/recipes", { pollIntervalMs: 30000 });

  // Mirror the shape the page expects: recipes is a computed from polling.data
  const recipes = computed<Recipe[]>(() => polling.data.value ?? []);

  return {
    recipes,
    loading: polling.loading,
    error: polling.error,
    isStale: polling.isStale,
    start: polling.start,
    stop: polling.stop,
    refresh: polling.refresh,
  };
};
```

- [ ] **Step 3: Verify recipes tests still pass**

```bash
cd src/CookHomie.Web
npm test -- tests/recipes-list.spec.ts tests/usePollingFetch.spec.ts
```
Expected: PASS.

- [ ] **Step 4: Refactor useInventory — preserve mutation method**

Replace `src/CookHomie.Web/composables/useInventory.ts` — keep `addInventoryItem` but delegate polling to `usePollingFetch`. Preserve `startPolling`, `stopPolling`, `refresh`, `loadInventory` names for page compatibility:

```ts
import type { AddInventoryItemPayload, InventoryItem } from "../types";

export const useInventory = () => {
  const polling = usePollingFetch<InventoryItem[]>("/api/inventory", { pollIntervalMs: 30000 });

  const items = computed<InventoryItem[]>(() => polling.data.value ?? []);

  const addInventoryItem = async (payload: AddInventoryItemPayload): Promise<InventoryItem> => {
    polling.loading.value = true;
    polling.error.value = null;
    try {
      const created = await $fetch<InventoryItem>("/api/inventory", {
        method: "POST",
        body: payload,
      });
      // Append to local cache
      polling.data.value = [created, ...(polling.data.value ?? [])];
      return created;
    } catch (err) {
      polling.error.value = err instanceof Error ? err.message : "Failed to add inventory item";
      throw err;
    } finally {
      polling.loading.value = false;
    }
  };

  return {
    items,
    loading: polling.loading,
    error: polling.error,
    isStale: polling.isStale,
    loadInventory: polling.refresh,
    startPolling: polling.start,
    stopPolling: polling.stop,
    refresh: polling.refresh,
    addInventoryItem,
  };
};
```

- [ ] **Step 5: Refactor useShoppingList — preserve mutation method**

Same pattern as `useInventory` for `useShoppingList.ts`. Preserve `startPolling`, `stopPolling`, `refresh`, `loadList`, `addToList`:

```ts
import type { ShoppingItem, AddShoppingItemPayload } from "../types";

export const useShoppingList = () => {
  const polling = usePollingFetch<ShoppingItem[]>("/api/shopping", { pollIntervalMs: 30000 });

  const items = computed<ShoppingItem[]>(() => polling.data.value ?? []);

  const addToList = async (ingredientNames: string[]): Promise<ShoppingItem[]> => {
    polling.loading.value = true;
    polling.error.value = null;
    try {
      const created = await $fetch<ShoppingItem[]>("/api/shopping", {
        method: "POST",
        body: { ingredientNames },
      });
      polling.data.value = [...(polling.data.value ?? []), ...created];
      return created;
    } catch (err) {
      polling.error.value = err instanceof Error ? err.message : "Failed to add items to shopping list";
      throw err;
    } finally {
      polling.loading.value = false;
    }
  };

  return {
    items,
    loading: polling.loading,
    error: polling.error,
    isStale: polling.isStale,
    loadList: polling.refresh,
    startPolling: polling.start,
    stopPolling: polling.stop,
    refresh: polling.refresh,
    addToList,
  };
};
```

- [ ] **Step 6: Run focused tests**

```bash
cd src/CookHomie.Web
npm test -- tests/useInventory.spec.ts tests/recipes-list.spec.ts tests/usePollingFetch.spec.ts
```
Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add src/CookHomie.Web/composables/useRecipes.ts src/CookHomie.Web/composables/useInventory.ts src/CookHomie.Web/composables/useShoppingList.ts
git commit -m "fix: delegate polling to usePollingFetch for visibility-aware behavior"
```

---

## Task 3: Fix recipeStore.addRecipe to Persist Ingredients

### Problem
`addRecipe` in `recipeStore.ts` spreads `data` but discards `ingredients`. The plan spec required mapping incoming ingredient objects into proper `RecipeIngredient` entries with generated ids.

### Files
- Modify: `src/CookHomie.Web/server/utils/recipeStore.ts`
- Test: `src/CookHomie.Web/tests/recipe-store.spec.ts`

### Steps

- [ ] **Step 1: Write failing test**

Add to `src/CookHomie.Web/tests/recipe-store.spec.ts`:

```ts
it("persists ingredients when adding a recipe", async () => {
  const { addRecipe } = await import("../server/utils/recipeStore");
  const recipe = addRecipe({
    name: "Garlic Toast",
    instructions: "Toast bread.",
    prepMinutes: 2,
    cookMinutes: 5,
    tags: ["snack"],
    ingredients: [
      { ingredientName: "Bread", quantity: 2, unit: "slices", isOptional: false },
      { ingredientName: "Garlic", quantity: 1, unit: "clove", isOptional: true },
    ],
  });

  expect(recipe.ingredients).toHaveLength(2);
  expect(recipe.ingredients[0]).toMatchObject({ ingredientName: "Bread", recipeId: recipe.id });
  expect(recipe.ingredients[1]).toMatchObject({ ingredientName: "Garlic", isOptional: true });
});
```

- [ ] **Step 2: Run test to verify failure**

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-store.spec.ts
```
Expected: FAIL — `recipe.ingredients` is undefined or empty.

- [ ] **Step 3: Fix addRecipe to map ingredients**

Replace the `addRecipe` function in `src/CookHomie.Web/server/utils/recipeStore.ts`:

```ts
export const addRecipe = (data: {
  name: string;
  instructions: string;
  prepMinutes: number;
  cookMinutes: number;
  tags: string[];
  ingredients: Array<{ ingredientName: string; quantity: number; unit: string; isOptional: boolean }>;
}): Recipe => {
  const id = `r${nextId++}`;
  const ingredients: RecipeIngredient[] = data.ingredients.map((ing, index) => ({
    id: `${id}-i${index + 1}`,
    recipeId: id,
    ingredientName: ing.ingredientName,
    quantity: ing.quantity,
    unit: ing.unit,
    isOptional: ing.isOptional,
  }));

  const recipe: Recipe = { ...data, id, ingredients };
  recipes.push(recipe);
  return recipe;
};
```

Also update the `getMissingIngredients` stub to accept the new signature (no logic change needed):

```ts
export const getMissingIngredients = (_recipeId: string, _inventoryNames: string[]): string[] => {
  return [];
};
```

- [ ] **Step 4: Run focused tests**

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-store.spec.ts tests/server-mock-audit.spec.ts
```
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/server/utils/recipeStore.ts src/CookHomie.Web/tests/recipe-store.spec.ts
git commit -m "fix: persist ingredients in recipeStore.addRecipe"
```

---

## Task 4: Verify AddRecipeModal Wires Ingredients to the Body

### Problem
The subagent reported completion of Task 5 but the code review could not confirm the modal's form state, submit body, and validation are wired to send `ingredients` to the API.

### Files
- Read: `src/CookHomie.Web/components/recipes/AddRecipeModal.vue`
- Test: `src/CookHomie.Web/tests/add-recipe-modal.spec.ts`

### Steps

- [ ] **Step 1: Read the modal file**

Read `src/CookHomie.Web/components/recipes/AddRecipeModal.vue` in full. Check for these conditions:

**Form reactive state includes `ingredients`:**
```ts
const form = reactive({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  instructions: "",
  tags: [] as string[],
  ingredients: [{ ingredientName: "", quantity: 1, unit: "", isOptional: false }],
});
```

**Add/remove ingredient helpers exist:**
```ts
const addIngredient = () => { form.ingredients.push(...); };
const removeIngredient = (index: number) => { form.ingredients.splice(index, 1); };
```

**Validation includes ingredients:**
```ts
const errors = reactive({ name: "", instructions: "", ingredients: "" });
// and a check that at least one ingredient name is non-empty
```

**Submit body includes ingredients:**
```ts
body: {
  ...form,
  ingredients: validIngredients.value.map(...),
},
```

**Template renders ingredient inputs** (at least name, quantity, unit, isOptional fields for each row).

- [ ] **Step 2: Run existing modal tests**

```bash
cd src/CookHomie.Web
npm test -- tests/add-recipe-modal.spec.ts
```
Expected: PASS.

- [ ] **Step 3: If any check in Step 1 fails — fix it**

If the form state, helpers, validation, submit body, or template are missing, fix them now following the spec plan template in `docs/superpowers/plans/2026-05-02-fix-milestone-2-frontend-gaps.md` Task 5 steps 5 (UI state and controls), step 6 (body wiring).

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/components/recipes/AddRecipeModal.vue src/CookHomie.Web/tests/add-recipe-modal.spec.ts
git commit -m "fix: verify AddRecipeModal ingredient capture wiring"
```

---

## Task 5: Full Verification

### Steps

- [ ] **Step 1: Run all frontend tests**

```bash
cd src/CookHomie.Web
npm test
```
Expected: all test files PASS.

- [ ] **Step 2: Run frontend validation (typecheck + lint + build)**

```bash
cd src/CookHomie.Web
npm run validate
```
Expected: typecheck, lint, build all exit 0.

- [ ] **Step 3: Update roadmap if all checks pass**

In `docs/04-Roadmap.md`, find the audit note added during the first pass:

```md
Milestone 2 audit note (2026-05-02): most frontend shell and mock-contract work is implemented and tested, but the dashboard and recipe detail items remain partially complete against the design intent.
```

Replace it with:

```md
Milestone 2 audit follow-up (2026-05-02): frontend gaps from the audit were fixed and verified with unit tests, validation, and responsive manual checks.
```

- [ ] **Step 4: Commit verification**

```bash
git add docs/04-Roadmap.md
git commit -m "docs: mark milestone 2 frontend gaps fully resolved"
```

---

## Self-Review

- **Spec coverage:** All 4 gaps have tasks: dashboard data contracts (Task 1), timer delegation (Task 2), ingredient persistence (Task 3), modal wiring verification (Task 4), final verification (Task 5).
- **Placeholder scan:** No `TBD`, `TODO`, or unspecified steps. All code blocks are concrete.
- **Type consistency:** `DashboardSummary.expiringItems: DashboardExpiringItem[]`, `DashboardSummary.recipeIdeas: DashboardRecipeIdea[]`, `AddRecipePayload.ingredients: AddRecipeIngredientPayload[]` — all introduced in Task 1 before being used in the page or store.
- **No step skipped:** Each task has test → fail → fix → pass → commit sequence.
- **Order is correct:** Task 1 (contracts) before Task 4 (modal uses the types). Task 2 (composables) is independent of Tasks 1/3/4. Task 5 last.

---

## Execution Recommendation

Tasks 1 and 3 are independent. Task 2 is independent from 1 and 3. Task 4 depends on Task 1's type changes. Task 5 is last.

**Order:** Task 1 → Task 3 → Task 2 → Task 4 → Task 5.

Or with subagent-driven: dispatch Tasks 1, 2, 3, 4 in sequence (each needs the types from Task 1 before it can be verified).
