# Fix Milestone 2 Frontend Gaps Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Close the Milestone 2 frontend gaps found in the audit: dashboard panel content, recipe-detail inventory loading, active-visible polling, and ingredient entry for added recipes.

**Architecture:** Keep business-facing frontend state in composables and utility functions, with Nuxt `server/api` routes acting as temporary mock adapters until C# endpoints exist. UI components stay humble: pages render state and trigger actions; matching, polling, and mock persistence logic live outside page templates.

**Tech Stack:** Nuxt 3, Vue 3 Composition API, TypeScript, Vitest, Vue Test Utils, Nuxt server API mock routes, Tailwind v4 CSS-first utilities.

---

## Scope and Parallelization

After Task 1 updates shared contracts, Tasks 2-5 are mostly independent and can be dispatched with `subagent-driven-development`:

- Task 2: Dashboard rows and panel content
- Task 3: Recipe-detail inventory loading
- Task 4: Visible-page polling lifecycle
- Task 5: AddRecipeModal ingredient capture and mock persistence

Task 6 must run last.

## Files to Touch

- Modify: `src/CookHomie.Web/types/index.ts` — expand temporary mock DTOs for dashboard rows and add-recipe ingredients.
- Modify: `src/CookHomie.Web/server/api/dashboard/summary.get.ts` — return panel row data in mock summary.
- Modify: `src/CookHomie.Web/server/utils/recipeStore.ts` — persist ingredients from `POST /api/recipes`.
- Modify: `src/CookHomie.Web/components/recipes/AddRecipeModal.vue` — add ingredient fields and submit ingredient payload.
- Modify: `src/CookHomie.Web/pages/index.vue` — render expiring items and recipe ideas in dashboard panels.
- Modify: `src/CookHomie.Web/composables/useRecipeDetail.ts` — fetch inventory before enriching ingredient stock state.
- Modify: `src/CookHomie.Web/composables/usePollingFetch.ts` — pause polling while page is hidden and resume when visible.
- Modify: `src/CookHomie.Web/composables/useRecipes.ts`, `src/CookHomie.Web/composables/useInventory.ts`, `src/CookHomie.Web/composables/useShoppingList.ts` — share visible-page polling behavior or delegate to `usePollingFetch` consistently.
- Test: `src/CookHomie.Web/tests/dashboard.spec.ts`, `src/CookHomie.Web/tests/index.spec.ts`, `src/CookHomie.Web/tests/usePollingFetch.spec.ts`, `src/CookHomie.Web/tests/recipe-detail.spec.ts`, `src/CookHomie.Web/tests/add-recipe-modal.spec.ts`, `src/CookHomie.Web/tests/recipe-store.spec.ts`, `src/CookHomie.Web/tests/recipe-types.spec.ts`.
- Modify: `docs/04-Roadmap.md` — mark Milestone 2 items complete only after tests and manual verification pass.

---

### Task 1: Expand Mock Contracts

**Files:**
- Modify: `src/CookHomie.Web/types/index.ts`
- Modify: `src/CookHomie.Web/server/api/dashboard/summary.get.ts`
- Test: `src/CookHomie.Web/tests/recipe-types.spec.ts`

- [ ] **Step 1: Write failing contract tests**

Add this to `src/CookHomie.Web/tests/recipe-types.spec.ts`:

```ts
import type { AddRecipePayload, DashboardSummary } from "../types";

it("allows dashboard summaries to carry panel row data", () => {
  const summary: DashboardSummary = {
    expiringCount: 1,
    recipeMatchCount: 1,
    shoppingCount: 2,
    expiringItems: [{ id: "inv-1", name: "Milk", expiresAt: "2026-05-03", location: "Fridge" }],
    recipeIdeas: [{ id: "r1", name: "Pancakes", matchedCount: 2, missingCount: 1 }],
  };

  expect(summary.expiringItems[0].name).toBe("Milk");
  expect(summary.recipeIdeas[0].matchedCount).toBe(2);
});

it("allows add recipe payloads to include ingredients", () => {
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

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-types.spec.ts
```

Expected: FAIL with TypeScript errors that `expiringItems`, `recipeIdeas`, or `ingredients` are not assignable to current types.

- [ ] **Step 3: Update shared types**

Change `src/CookHomie.Web/types/index.ts` dashboard and add-recipe types to:

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
  expiringItems: DashboardExpiringItem[];
  recipeIdeas: DashboardRecipeIdea[];
}

export type AddRecipeIngredientPayload = Omit<RecipeIngredient, "id" | "recipeId" | "isInStock">;

export type AddRecipePayload = Omit<Recipe, "id" | "ingredients"> & {
  ingredients: AddRecipeIngredientPayload[];
};
```

- [ ] **Step 4: Update dashboard mock response**

Change `src/CookHomie.Web/server/api/dashboard/summary.get.ts` to:

```ts
// TEMP_MOCK: No C# API endpoint exists yet for GET /api/dashboard/summary.
// Replace this route with a proxy when backend DashboardController exists.
import type { DashboardSummary } from "~/types";

export default defineEventHandler((): DashboardSummary => ({
  expiringCount: 3,
  recipeMatchCount: 2,
  shoppingCount: 4,
  expiringItems: [
    { id: "inv-1", name: "Milk", expiresAt: "2026-05-03", location: "Fridge" },
    { id: "inv-2", name: "Spinach", expiresAt: "2026-05-04", location: "Fridge" },
    { id: "inv-3", name: "Yogurt", expiresAt: "2026-05-05", location: "Fridge" },
  ],
  recipeIdeas: [
    { id: "r1", name: "Classic Pancakes", matchedCount: 2, missingCount: 1 },
    { id: "r3", name: "Avocado Toast", matchedCount: 2, missingCount: 0 },
  ],
}));
```

- [ ] **Step 5: Run focused tests**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-types.spec.ts tests/server-mock-audit.spec.ts
```

Expected: PASS for both files.

- [ ] **Step 6: Commit**

```bash
git add src/CookHomie.Web/types/index.ts src/CookHomie.Web/server/api/dashboard/summary.get.ts src/CookHomie.Web/tests/recipe-types.spec.ts
git commit -m "fix: expand frontend mock contracts"
```

---

### Task 2: Render Dashboard Panel Rows

**Files:**
- Modify: `src/CookHomie.Web/pages/index.vue`
- Test: `src/CookHomie.Web/tests/index.spec.ts`

- [ ] **Step 1: Write failing dashboard page test**

Add this test to `src/CookHomie.Web/tests/index.spec.ts`:

```ts
it("renders expiring items and recipe ideas in dashboard panels", () => {
  vi.stubGlobal("useDashboard", () => ({
    data: ref({
      expiringCount: 1,
      recipeMatchCount: 1,
      shoppingCount: 0,
      expiringItems: [{ id: "inv-1", name: "Milk", expiresAt: "2026-05-03", location: "Fridge" }],
      recipeIdeas: [{ id: "r1", name: "Classic Pancakes", matchedCount: 2, missingCount: 1 }],
    }),
    loading: ref(false),
    error: ref(null),
    isStale: ref(false),
    start: vi.fn(),
    stop: vi.fn(),
    refresh: vi.fn(),
  }));

  const wrapper = mount(IndexPage, {
    global: {
      stubs: {
        DashboardStatCard: {
          template: '<div class="stat-card"><span>{{ label }}</span><slot /></div>',
          props: ["label", "value", "sub", "variant"],
        },
        DashboardPanel: {
          template: '<section class="panel"><h2>{{ title }}</h2><slot /></section>',
          props: ["title", "loading", "error", "isStale", "hasData", "showRefresh"],
        },
      },
    },
  });

  expect(wrapper.text()).toContain("Milk");
  expect(wrapper.text()).toContain("Fridge");
  expect(wrapper.text()).toContain("Classic Pancakes");
  expect(wrapper.text()).toContain("2 in stock");
  expect(wrapper.text()).toContain("1 missing");
});
```

- [ ] **Step 2: Run test to verify failure**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/index.spec.ts
```

Expected: FAIL because `Milk` and `Classic Pancakes` are not rendered.

- [ ] **Step 3: Render expiring item rows and recipe idea rows**

In `src/CookHomie.Web/pages/index.vue`, replace the first panel slot with:

```vue
<ul v-if="(data?.expiringItems.length ?? 0) > 0" class="list-none p-0 m-0 flex flex-col gap-3">
  <li
    v-for="item in data?.expiringItems"
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
```

Replace the second panel slot with:

```vue
<ul v-if="(data?.recipeIdeas.length ?? 0) > 0" class="list-none p-0 m-0 flex flex-col gap-3">
  <li
    v-for="recipe in data?.recipeIdeas"
    :key="recipe.id"
    class="rounded-md border border-border bg-bg p-3"
  >
    <NuxtLink :to="`/recipes/${recipe.id}`" class="font-semibold text-text-primary no-underline">
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
```

Add this helper to the `<script setup>` block:

```ts
const formatExpiry = (date: string) => {
  const d = new Date(date);
  const now = new Date();
  const diff = Math.ceil((d.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
  if (diff < 0) return "Expired";
  if (diff === 0) return "Expires today";
  if (diff <= 3) return `Expires in ${diff}d`;
  return d.toLocaleDateString();
};
```

- [ ] **Step 4: Run focused tests**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/index.spec.ts tests/dashboard.spec.ts tests/dashboard-components.spec.ts
```

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/pages/index.vue src/CookHomie.Web/tests/index.spec.ts
git commit -m "fix: render dashboard panel content"
```

---

### Task 3: Load Inventory for Recipe Detail Stock Badges

**Files:**
- Modify: `src/CookHomie.Web/composables/useRecipeDetail.ts`
- Test: `src/CookHomie.Web/tests/recipe-detail.spec.ts`

- [ ] **Step 1: Write failing composable test**

Append this to `src/CookHomie.Web/tests/recipe-detail.spec.ts`:

```ts
import { ref } from "vue";
import { vi } from "vitest";

vi.mock("../composables/useInventory", () => ({
  useInventory: () => ({
    items: ref([{ id: "inv-1", name: "Milk" }]),
    loadInventory: vi.fn().mockResolvedValue(undefined),
  }),
}));

it("loads inventory before enriching recipe ingredients", async () => {
  vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));
  vi.stubGlobal("$fetch", vi.fn().mockResolvedValue({
    id: "r1",
    name: "Pancakes",
    instructions: "Mix.",
    prepMinutes: 5,
    cookMinutes: 10,
    tags: [],
    ingredients: [
      { id: "i1", recipeId: "r1", ingredientName: "milk", quantity: 250, unit: "ml", isOptional: false },
    ],
  }));

  const { useRecipeDetail } = await import("../composables/useRecipeDetail");
  const detail = useRecipeDetail("r1");
  await detail.start();

  expect(detail.enrichedIngredients.value[0].isInStock).toBe(true);
});
```

- [ ] **Step 2: Run test to verify failure**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-detail.spec.ts
```

Expected: FAIL because `useRecipeDetail.start()` does not call `loadInventory()` and the mock import shape may expose the missing behavior.

- [ ] **Step 3: Fetch inventory before recipe enrichment path**

Change `src/CookHomie.Web/composables/useRecipeDetail.ts` to destructure `loadInventory` and call both fetches in `start`:

```ts
const { items: inventoryItems, loadInventory } = useInventory();

const start = async () => {
  await Promise.all([loadInventory(), fetchRecipe()]);
};
const stop = () => {};
```

Keep the existing `inventoryNames` computed and `matchIngredientStock()` exact normalized check unchanged.

- [ ] **Step 4: Run focused tests**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-detail.spec.ts tests/ingredient-stock.spec.ts
```

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/composables/useRecipeDetail.ts src/CookHomie.Web/tests/recipe-detail.spec.ts
git commit -m "fix: load inventory for recipe stock badges"
```

---

### Task 4: Pause Polling While Page Hidden

**Files:**
- Modify: `src/CookHomie.Web/composables/usePollingFetch.ts`
- Modify: `src/CookHomie.Web/composables/useRecipes.ts`
- Modify: `src/CookHomie.Web/composables/useInventory.ts`
- Modify: `src/CookHomie.Web/composables/useShoppingList.ts`
- Test: `src/CookHomie.Web/tests/usePollingFetch.spec.ts`, `src/CookHomie.Web/tests/useInventory.spec.ts`, `src/CookHomie.Web/tests/recipes-list.spec.ts`

- [ ] **Step 1: Write failing visibility test**

Add this to `src/CookHomie.Web/tests/usePollingFetch.spec.ts`:

```ts
it("pauses polling while document is hidden and resumes when visible", async () => {
  const listeners: Record<string, EventListener> = {};
  Object.defineProperty(document, "hidden", { configurable: true, value: false });
  vi.spyOn(document, "addEventListener").mockImplementation((event, listener) => {
    listeners[event] = listener as EventListener;
  });
  vi.spyOn(document, "removeEventListener").mockImplementation(() => {});

  const spy = vi.fn().mockResolvedValue({ data: "value" });
  vi.stubGlobal("$fetch", spy);

  const { start, stop } = usePollingFetch("/api/test");
  await start();
  spy.mockClear();

  Object.defineProperty(document, "hidden", { configurable: true, value: true });
  listeners.visibilitychange(new Event("visibilitychange"));
  await vi.advanceTimersByTimeAsync(30000);
  expect(spy).not.toHaveBeenCalled();

  Object.defineProperty(document, "hidden", { configurable: true, value: false });
  listeners.visibilitychange(new Event("visibilitychange"));
  expect(spy).toHaveBeenCalledTimes(1);

  stop();
});
```

- [ ] **Step 2: Run test to verify failure**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/usePollingFetch.spec.ts
```

Expected: FAIL because no `visibilitychange` listener is registered.

- [ ] **Step 3: Implement visibility-aware polling once**

Update `src/CookHomie.Web/composables/usePollingFetch.ts` with these helpers inside `usePollingFetch`:

```ts
let intervalId: ReturnType<typeof setInterval> | null = null;
let started = false;

const clearTimer = () => {
  if (intervalId !== null) {
    clearInterval(intervalId);
    intervalId = null;
  }
};

const schedule = () => {
  clearTimer();
  if (typeof document !== "undefined" && document.hidden) return;
  intervalId = setInterval(fetchData, pollIntervalMs);
};

const handleVisibilityChange = async () => {
  if (!started) return;
  if (document.hidden) {
    clearTimer();
    return;
  }
  await fetchData();
  schedule();
};
```

Then change `start` and `stop` to:

```ts
const start = async () => {
  if (started) return;
  started = true;
  if (typeof document !== "undefined") {
    document.addEventListener("visibilitychange", handleVisibilityChange);
  }
  await fetchData();
  schedule();
};

const stop = () => {
  started = false;
  clearTimer();
  if (typeof document !== "undefined") {
    document.removeEventListener("visibilitychange", handleVisibilityChange);
  }
};
```

- [ ] **Step 4: Align list composables with shared polling behavior**

Refactor `useRecipes.ts`, `useInventory.ts`, and `useShoppingList.ts` to either call `usePollingFetch` or copy the same visibility listener pattern. Preferred implementation for `useRecipes.ts`:

```ts
export const useRecipes = () => {
  const polling = usePollingFetch<Recipe[]>("/api/recipes", { pollIntervalMs: 30000 });
  const recipes = computed(() => polling.data.value ?? []);

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

For `useInventory.ts` and `useShoppingList.ts`, keep mutation methods (`addInventoryItem`, `addToList`) and replace only timer ownership with `usePollingFetch<T[]>`. Preserve returned method names `startPolling`, `stopPolling`, `refresh`, `loadInventory`, and `loadList` so pages do not change.

- [ ] **Step 5: Run focused tests**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/usePollingFetch.spec.ts tests/useInventory.spec.ts tests/recipes-list.spec.ts
```

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/CookHomie.Web/composables/usePollingFetch.ts src/CookHomie.Web/composables/useRecipes.ts src/CookHomie.Web/composables/useInventory.ts src/CookHomie.Web/composables/useShoppingList.ts src/CookHomie.Web/tests/usePollingFetch.spec.ts src/CookHomie.Web/tests/useInventory.spec.ts src/CookHomie.Web/tests/recipes-list.spec.ts
git commit -m "fix: pause polling when page hidden"
```

---

### Task 5: Capture Ingredients in AddRecipeModal

**Files:**
- Modify: `src/CookHomie.Web/components/recipes/AddRecipeModal.vue`
- Modify: `src/CookHomie.Web/server/utils/recipeStore.ts`
- Test: `src/CookHomie.Web/tests/add-recipe-modal.spec.ts`, `src/CookHomie.Web/tests/recipe-store.spec.ts`

- [ ] **Step 1: Write failing recipe store test**

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
});
```

- [ ] **Step 2: Write failing modal submit test**

In `src/CookHomie.Web/tests/add-recipe-modal.spec.ts`, update the valid form test to set one ingredient and assert request body:

```ts
vm.form.ingredients = [{ ingredientName: "Bread", quantity: 2, unit: "slices", isOptional: false }];
await vm.handleSubmit();

expect($fetchMock).toHaveBeenCalledWith("/api/recipes", {
  method: "POST",
  body: expect.objectContaining({
    name: "Test Recipe",
    ingredients: [{ ingredientName: "Bread", quantity: 2, unit: "slices", isOptional: false }],
  }),
});
```

- [ ] **Step 3: Run tests to verify failure**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-store.spec.ts tests/add-recipe-modal.spec.ts
```

Expected: FAIL because mock store discards ingredients and modal form has no `ingredients` field.

- [ ] **Step 4: Persist ingredients in mock store**

Update `src/CookHomie.Web/server/utils/recipeStore.ts` imports and `addRecipe`:

```ts
import type { AddRecipePayload, Recipe, RecipeIngredient } from "~/types";
```

```ts
export const addRecipe = (data: AddRecipePayload): Recipe => {
  const id = `r${nextId++}`;
  const ingredients = data.ingredients.map((ingredient, index): RecipeIngredient => ({
    id: `${id}-i${index + 1}`,
    recipeId: id,
    ingredientName: ingredient.ingredientName,
    quantity: ingredient.quantity,
    unit: ingredient.unit,
    isOptional: ingredient.isOptional,
  }));

  const recipe: Recipe = { ...data, id, ingredients };
  recipes.push(recipe);
  return recipe;
};
```

- [ ] **Step 5: Add ingredient UI state and controls**

In `src/CookHomie.Web/components/recipes/AddRecipeModal.vue`, change `form` to include ingredients:

```ts
const form = reactive({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  instructions: "",
  tags: [] as string[],
  ingredients: [
    { ingredientName: "", quantity: 1, unit: "", isOptional: false },
  ],
});
```

Add helpers:

```ts
const addIngredient = () => {
  form.ingredients.push({ ingredientName: "", quantity: 1, unit: "", isOptional: false });
};

const removeIngredient = (index: number) => {
  if (form.ingredients.length === 1) {
    form.ingredients[0] = { ingredientName: "", quantity: 1, unit: "", isOptional: false };
    return;
  }
  form.ingredients.splice(index, 1);
};
```

Extend validation:

```ts
const errors = reactive({ name: "", instructions: "", ingredients: "" });

const validIngredients = computed(() =>
  form.ingredients.filter((ingredient) => ingredient.ingredientName.trim())
);

if (validIngredients.value.length === 0) {
  errors.ingredients = "At least one ingredient is required";
  isValid = false;
} else {
  errors.ingredients = "";
}
```

Before submit, send trimmed ingredients only:

```ts
body: {
  ...form,
  ingredients: validIngredients.value.map((ingredient) => ({
    ingredientName: ingredient.ingredientName.trim(),
    quantity: Number(ingredient.quantity) || 0,
    unit: ingredient.unit.trim(),
    isOptional: ingredient.isOptional,
  })),
},
```

Add this template block before Tags:

```vue
<SharedFormField label="Ingredients" :error="errors.ingredients" required>
  <div class="flex flex-col gap-3">
    <div
      v-for="(ingredient, index) in form.ingredients"
      :key="index"
      class="grid grid-cols-[1fr_90px_100px_auto_auto] gap-2 max-md:grid-cols-1"
    >
      <input v-model="ingredient.ingredientName" class="input" placeholder="Ingredient" >
      <input v-model.number="ingredient.quantity" type="number" min="0" step="0.01" class="input" placeholder="Qty" >
      <input v-model="ingredient.unit" class="input" placeholder="Unit" >
      <label class="flex items-center gap-2 text-sm text-text-secondary">
        <input v-model="ingredient.isOptional" type="checkbox" > Optional
      </label>
      <SharedButton variant="secondary" @click="removeIngredient(index)">Remove</SharedButton>
    </div>
    <SharedButton variant="secondary" @click="addIngredient">+ Add Ingredient</SharedButton>
  </div>
</SharedFormField>
```

- [ ] **Step 6: Run focused tests**

Run:

```bash
cd src/CookHomie.Web
npm test -- tests/recipe-store.spec.ts tests/add-recipe-modal.spec.ts
```

Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add src/CookHomie.Web/components/recipes/AddRecipeModal.vue src/CookHomie.Web/server/utils/recipeStore.ts src/CookHomie.Web/tests/add-recipe-modal.spec.ts src/CookHomie.Web/tests/recipe-store.spec.ts
git commit -m "fix: add ingredients to recipe creation"
```

---

### Task 6: Full Verification and Roadmap Update

**Files:**
- Modify: `docs/04-Roadmap.md`

- [ ] **Step 1: Run full frontend tests**

Run:

```bash
cd src/CookHomie.Web
npm test
```

Expected: all Vitest files PASS.

- [ ] **Step 2: Run frontend validation**

Run:

```bash
cd src/CookHomie.Web
npm run validate
```

Expected: typecheck, lint, and build all exit 0.

- [ ] **Step 3: Manual responsive check**

Run:

```bash
cd src/CookHomie.Web
npm run dev
```

Open `http://localhost:3000` and verify:

- Phone width: dashboard cards stack, dashboard panel rows fit without horizontal scroll, recipe add ingredient rows stack.
- Tablet width: recipe cards and dashboard panels use available width without clipped actions.
- Desktop width: dashboard has three stat cards and two panel columns.
- Recipe detail direct load: ingredients show accurate `In stock` when matching inventory exists.
- Background tab: polling stops while hidden and refreshes once visible.

- [ ] **Step 4: Update roadmap**

If verification passes, change `docs/04-Roadmap.md` Milestone 2 items back to complete:

```md
- [x] Dashboard page (expiring items, recipe matches, shopping count)
- [x] Recipe detail page (ingredients with in-stock highlighting)
```

Replace the audit note with:

```md
Milestone 2 audit follow-up (2026-05-02): frontend gaps from the audit were fixed and verified with unit tests, validation, and responsive manual checks.
```

- [ ] **Step 5: Commit verification docs**

```bash
git add docs/04-Roadmap.md
git commit -m "docs: mark milestone 2 frontend gaps fixed"
```

---

## Self-Review

- Spec coverage: dashboard panel content, recipe-detail stock loading, visible-page polling, AddRecipe ingredients, tests, and roadmap update are covered.
- Placeholder scan: no `TBD`, `TODO`, or unspecified test/write steps remain.
- Type consistency: `DashboardSummary.expiringItems`, `DashboardSummary.recipeIdeas`, and `AddRecipePayload.ingredients` are introduced before later tasks use them.
