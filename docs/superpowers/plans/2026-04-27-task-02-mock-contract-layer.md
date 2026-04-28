# Milestone 2 — Mock Contract Layer

> **For agentic workers:** Each step is `- [ ]`. Implement task-by-task on its own branch. Commit after each task.

**Goal:** Build the Nuxt server/api mock contract layer (in-memory recipe store + mock endpoints) and the client-side ingredient stock matching utility.

**Prerequisite:** Complete Task 1 (design tokens exist) before starting. The mock endpoints use CSS vars from tokens.css.

---

## Task 7 — In-memory recipe store + mock API endpoints

**Files:**
- Create: `src/CookHomie.Web/server/utils/recipeStore.ts`
- Create: `src/CookHomie.Web/server/api/recipes/index.get.ts`
- Create: `src/CookHomie.Web/server/api/recipes/index.post.ts`
- Create: `src/CookHomie.Web/server/api/recipes/[id].get.ts`
- Create: `src/CookHomie.Web/server/api/recipes/[id]/missing.get.ts`
- Create: `src/CookHomie.Web/server/api/dashboard/summary.get.ts`
- Create: `src/CookHomie.Web/server/tests/recipe-store.spec.ts`

- [ ] **Step 1: Write recipe store**

```typescript
// src/CookHomie.Web/server/utils/recipeStore.ts
import type { Recipe } from "~/types";

const recipes: Recipe[] = [
  {
    id: "r1",
    name: "Classic Pancakes",
    instructions: "Mix flour, eggs, milk. Cook on griddle.",
    prepMinutes: 5,
    cookMinutes: 10,
    tags: ["breakfast", "quick"]
  },
  {
    id: "r2",
    name: "Tomato Pasta",
    instructions: "Boil pasta. Sauté garlic, add tomatoes, toss.",
    prepMinutes: 5,
    cookMinutes: 15,
    tags: ["pasta", "vegetarian"]
  },
  {
    id: "r3",
    name: "Avocado Toast",
    instructions: "Toast bread. Mash avocado with salt and lemon. Spread.",
    prepMinutes: 3,
    cookMinutes: 2,
    tags: ["breakfast", "quick", "vegetarian"]
  }
];

let nextId = 4;

export const getRecipes = (): Recipe[] => recipes;

export const getRecipeById = (id: string): Recipe | undefined =>
  recipes.find(r => r.id === id);

export const addRecipe = (data: Omit<Recipe, "id">): Recipe => {
  const recipe: Recipe = { ...data, id: `r${nextId++}` };
  recipes.push(recipe);
  return recipe;
};

export const getMissingIngredients = (_recipeId: string, _inventoryNames: string[]): string[] => {
  // Milestone 2: stock matching done client-side; return empty
  return [];
};
```

- [ ] **Step 2: Write GET /api/recipes**

```typescript
// src/CookHomie.Web/server/api/recipes/index.get.ts
import { getRecipes } from "~/server/utils/recipeStore";
export default defineEventHandler(() => getRecipes());
```

- [ ] **Step 3: Write POST /api/recipes**

```typescript
// src/CookHomie.Web/server/api/recipes/index.post.ts
import { addRecipe } from "~/server/utils/recipeStore";
export default defineEventHandler(async (event) => {
  const body = await readBody(event);
  if (!body?.name || !body?.instructions) {
    throw createError({ statusCode: 400, statusMessage: "name and instructions are required" });
  }
  return addRecipe(body);
});
```

- [ ] **Step 4: Write GET /api/recipes/:id**

```typescript
// src/CookHomie.Web/server/api/recipes/[id].get.ts
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

- [ ] **Step 5: Write GET /api/recipes/:id/missing**

```typescript
// src/CookHomie.Web/server/api/recipes/[id]/missing.get.ts
import { getMissingIngredients } from "~/server/utils/recipeStore";
export default defineEventHandler((event) => {
  const id = getRouterParam(event, "id");
  return getMissingIngredients(id!, []);
});
```

- [ ] **Step 6: Write GET /api/dashboard/summary**

```typescript
// src/CookHomie.Web/server/api/dashboard/summary.get.ts
export default defineEventHandler(() => ({
  expiringCount: 3,
  recipeMatchCount: 7,
  shoppingCount: 4
}));
```

- [ ] **Step 7: Write store unit tests**

```typescript
// src/CookHomie.Web/server/tests/recipe-store.spec.ts
import { describe, expect, it } from "vitest";
import { getRecipes, getRecipeById, addRecipe } from "../../utils/recipeStore";

describe("recipeStore", () => {
  it("getRecipes returns array", () => {
    const recipes = getRecipes();
    expect(Array.isArray(recipes)).toBe(true);
    expect(recipes.length).toBeGreaterThan(0);
  });

  it("getRecipeById returns recipe or undefined", () => {
    const r = getRecipeById("r1");
    expect(r?.name).toBe("Classic Pancakes");
    expect(getRecipeById("nonexistent")).toBeUndefined();
  });

  it("addRecipe returns recipe with new id", () => {
    const before = getRecipes().length;
    const newR = addRecipe({ name: "Test", instructions: "Test", prepMinutes: 1, cookMinutes: 1, tags: [] });
    expect(newR.id).toBeTruthy();
    expect(getRecipes().length).toBe(before + 1);
  });
});
```

- [ ] **Step 8: Run tests — verify they pass**

Run: `cd src/CookHomie.Web && npx vitest run server/tests/recipe-store.spec.ts`
Expected: PASS

- [ ] **Step 9: Commit**

```bash
git add src/CookHomie.Web/server/utils/recipeStore.ts src/CookHomie.Web/server/api/recipes/ src/CookHomie.Web/server/api/dashboard/ src/CookHomie.Web/server/tests/
git commit -m "feat(web): add in-memory recipe store and mock API endpoints"
```

---

## Task 8 — Ingredient stock matching utility

**Files:**
- Create: `src/CookHomie.Web/utils/ingredientStock.ts`
- Create: `src/CookHomie.Web/tests/ingredient-stock.spec.ts`

- [ ] **Step 1: Write failing test**

```typescript
// src/CookHomie.Web/tests/ingredient-stock.spec.ts
import { describe, expect, it } from "vitest";
import { matchIngredientStock } from "../utils/ingredientStock";

describe("matchIngredientStock", () => {
  it("matches exact trimmed name case-insensitively", () => {
    const inventory = ["Milk", "Eggs", "Butter"];
    expect(matchIngredientStock("milk", inventory)).toBe(true);
    expect(matchIngredientStock("MILK", inventory)).toBe(true);
    expect(matchIngredientStock("  Milk  ", inventory)).toBe(true);
    expect(matchIngredientStock("eggs", inventory)).toBe(true);
  });

  it("returns false for non-matching ingredient", () => {
    const inventory = ["Milk", "Eggs"];
    expect(matchIngredientStock("Flour", inventory)).toBe(false);
  });

  it("handles empty inventory", () => {
    expect(matchIngredientStock("Milk", [])).toBe(false);
  });
});
```

- [ ] **Step 2: Run test — verify it fails**

Run: `cd src/CookHomie.Web && npx vitest run tests/ingredient-stock.spec.ts`
Expected: FAIL — matchIngredientStock not defined

- [ ] **Step 3: Write implementation**

```typescript
// src/CookHomie.Web/utils/ingredientStock.ts
export const matchIngredientStock = (
  ingredientName: string,
  inventoryNames: string[]
): boolean => {
  const normalized = ingredientName.trim().toLowerCase();
  return inventoryNames.some(n => n.trim().toLowerCase() === normalized);
};
```

- [ ] **Step 4: Run test — verify it passes**

Run: `cd src/CookHomie.Web && npx vitest run tests/ingredient-stock.spec.ts`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/utils/ingredientStock.ts src/CookHomie.Web/tests/ingredient-stock.spec.ts
git commit -m "feat(web): add ingredient stock matching utility"
```
