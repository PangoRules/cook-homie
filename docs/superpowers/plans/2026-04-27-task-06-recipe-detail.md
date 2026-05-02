# Milestone 2 — Recipe Detail

**Branch:** `task/recipe-detail`
**Parent branch:** `feat/milestone-2-frontend`

> **For agentic workers:** Each step is `- [ ]`. Implement task-by-task on its own branch. Commit after each task.

**Goal:** Build the full Recipe detail page with IngredientStockBadge component, useRecipeDetail composable, and in-stock highlighting using exact normalized string match.

**Prerequisite:** Complete Tasks 1–3, 6, 7, 8, 9, 12 (design tokens, shared primitives, types, mock endpoints, ingredient stock utility, CSS system, RecipeCard).

---

## Task 15 — IngredientStockBadge component

**Files:**
- Create: `src/CookHomie.Web/components/recipes/detail/IngredientStockBadge.vue`
- Create: `src/CookHomie.Web/tests/recipe-detail.spec.ts`

- [ ] **Step 1: Write IngredientStockBadge**

```vue
<!-- src/CookHomie.Web/components/recipes/detail/IngredientStockBadge.vue -->
<template>
  <span
    :class="[
      'inline-flex items-center gap-1 px-2 py-1 rounded-full text-[11px] font-semibold uppercase tracking-[0.05em]',
      isInStock
        ? 'bg-success-subtle text-success'
        : 'bg-error-subtle text-error'
    ]"
  >
    <span class="w-1.5 h-1.5 rounded-full bg-current" />
    {{ isInStock ? 'In stock' : 'Missing' }}
  </span>
</template>

<script setup lang="ts">
defineProps<{ isInStock: boolean }>();
</script>
```

- [ ] **Step 2: Write tests**

```typescript
// src/CookHomie.Web/tests/recipe-detail.spec.ts
import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import IngredientStockBadge from "../components/recipes/detail/IngredientStockBadge.vue";

describe("IngredientStockBadge", () => {
  it("shows 'In stock' when isInStock is true", () => {
    const wrapper = mount(IngredientStockBadge, { props: { isInStock: true } });
    expect(wrapper.text()).toContain("In stock");
    expect(wrapper.find(".bg-success-subtle").exists()).toBe(true);
  });

  it("shows 'Missing' when isInStock is false", () => {
    const wrapper = mount(IngredientStockBadge, { props: { isInStock: false } });
    expect(wrapper.text()).toContain("Missing");
    expect(wrapper.find(".bg-error-subtle").exists()).toBe(true);
  });
});
```

- [ ] **Step 3: Run tests**

Run: `cd src/CookHomie.Web && npx vitest run tests/recipe-detail.spec.ts`
Expected: PASS

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/components/recipes/detail/IngredientStockBadge.vue src/CookHomie.Web/tests/recipe-detail.spec.ts
git commit -m "feat(web): add IngredientStockBadge component"
```

---

## Task 16 — Recipe detail page + useRecipeDetail composable

**Files:**
- Modify: `src/CookHomie.Web/pages/recipes/[id].vue`
- Modify: `src/CookHomie.Web/server/utils/recipeStore.ts` (add sample ingredients)
- Create: `src/CookHomie.Web/composables/useRecipeDetail.ts`

- [ ] **Step 1: Update mock store with sample RecipeIngredient[] data**

Add a `RecipeIngredient[]` to at least one recipe in `recipeStore.ts` so in-stock highlighting is demonstrable:

```typescript
// src/CookHomie.Web/server/utils/recipeStore.ts
import type { Recipe, RecipeIngredient } from "~/types";
// ...
const recipes: Recipe[] = [
  {
    id: "r1",
    name: "Classic Pancakes",
    instructions: "Mix flour, eggs, milk. Cook on griddle.",
    prepMinutes: 5,
    cookMinutes: 10,
    tags: ["breakfast", "quick"],
    ingredients: [
      { id: "i1", recipeId: "r1", ingredientName: "flour",     quantity: 200, unit: "g",   isOptional: false },
      { id: "i2", recipeId: "r1", ingredientName: "eggs",      quantity: 2,   unit: "pcs", isOptional: false },
      { id: "i3", recipeId: "r1", ingredientName: "milk",      quantity: 250, unit: "ml",  isOptional: false },
      { id: "i4", recipeId: "r1", ingredientName: "butter",    quantity: 30,  unit: "g",   isOptional: true  },
    ] as RecipeIngredient[],
  },
  // r2, r3 unchanged
];
```

- [ ] **Step 2: Write useRecipeDetail**

```typescript
// src/CookHomie.Web/composables/useRecipeDetail.ts
import type { Recipe, RecipeIngredient } from "~/types";
import { matchIngredientStock } from "~/utils/ingredientStock";
import { useInventory } from "./useInventory";

export const useRecipeDetail = (recipeId: string) => {
  const recipe = useState<Recipe | null>(`recipe-${recipeId}`, () => null);
  const loading = useState(`recipe-loading-${recipeId}`, () => false);
  const error = useState<string | null>(`recipe-error-${recipeId}`, () => null);

  // Derive inventory names from the shared inventory state
  const { items: inventoryItems } = useInventory();
  const inventoryNames = computed(() =>
    inventoryItems.value.map(item => item.name)
  );

  const fetchRecipe = async () => {
    loading.value = true;
    error.value = null;
    try {
      recipe.value = await $fetch<Recipe>(`/api/recipes/${recipeId}`);
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load recipe";
    } finally {
      loading.value = false;
    }
  };

  const enrichedIngredients = computed<RecipeIngredient[]>(() => {
    if (!recipe.value) return [];
    return recipe.value.ingredients.map(ing => ({
      ...ing,
      isInStock: matchIngredientStock(ing.ingredientName, inventoryNames.value)
    }));
  });

  const start = () => fetchRecipe();
  const stop = () => {};

  return { recipe, loading, error, enrichedIngredients, start, stop };
};
```

- [ ] **Step 3: Implement full Recipe detail page**

Replace `src/CookHomie.Web/pages/recipes/[id].vue`:

```vue
<template>
  <div class="max-w-[720px] mx-auto">
    <div v-if="loading && !recipe">
      <SkeletonBlock height="40px" width="60%" class="mb-4" />
      <SkeletonBlock height="200px" />
    </div>
    <ErrorBanner v-else-if="error" :message="error" :show-retry="true" @retry="fetchRecipe" />

    <template v-else-if="recipe">
      <header class="mb-8">
        <NuxtLink to="/recipes" class="inline-block mb-4 text-accent no-underline text-sm">← Recipes</NuxtLink>
        <h1 class="font-display text-[32px] font-bold mb-3">{{ recipe.name }}</h1>
        <div class="flex gap-2 flex-wrap mb-3">
          <span
            v-for="tag in recipe.tags"
            :key="tag"
            class="bg-accent-subtle text-accent rounded-full px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.05em]"
          >{{ tag }}</span>
        </div>
        <p class="text-text-muted text-sm m-0">
          {{ recipe.prepMinutes + recipe.cookMinutes }} min total
          ({{ recipe.prepMinutes }} prep + {{ recipe.cookMinutes }} cook)
        </p>
      </header>

      <div class="flex flex-col gap-8">
        <section>
          <h2 class="font-display text-[20px] font-semibold mb-4 pb-2 border-b border-border">Instructions</h2>
          <p class="text-[15px] leading-[1.7] text-text-primary m-0">{{ recipe.instructions }}</p>
        </section>

        <section v-if="enrichedIngredients.length > 0">
          <h2 class="font-display text-[20px] font-semibold mb-4 pb-2 border-b border-border">Ingredients</h2>
          <ul class="list-none p-0 m-0 flex flex-col gap-3">
            <li
              v-for="ing in enrichedIngredients"
              :key="ing.id"
              class="flex items-center justify-between gap-3 p-3 bg-surface border border-border rounded-md"
            >
              <div class="flex flex-col">
                <span class="font-medium">{{ ing.ingredientName }}</span>
                <span v-if="ing.quantity" class="text-text-muted text-[13px]">
                  {{ ing.quantity }}{{ ing.unit }}
                </span>
              </div>
              <IngredientStockBadge :is-in-stock="ing.isInStock ?? false" />
            </li>
          </ul>
        </section>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
const route = useRoute();
const { recipe, loading, error, enrichedIngredients, start, fetchRecipe } = useRecipeDetail(route.params.id as string);

onMounted(() => start());
</script>
```

- [ ] **Step 5: Run tests**

Run: `cd src/CookHomie.Web && npx vitest run tests/recipe-detail.spec.ts`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add src/CookHomie.Web/pages/recipes/[id].vue src/CookHomie.Web/composables/useRecipeDetail.ts src/CookHomie.Web/server/utils/recipeStore.ts
git commit -m "feat(web): implement recipe detail page with IngredientStockBadge and in-stock highlighting"
```
