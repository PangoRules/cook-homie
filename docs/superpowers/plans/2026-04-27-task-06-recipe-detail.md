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
  <span :class="['stock-badge', isInStock ? 'stock-badge--in' : 'stock-badge--out']">
    <span class="stock-badge__dot" />
    {{ isInStock ? "In stock" : "Missing" }}
  </span>
</template>

<script setup lang="ts">
defineProps<{ isInStock: boolean }>();
</script>

<style scoped>
.stock-badge {
  display: inline-flex;
  align-items: center;
  gap: var(--space-1);
  padding: var(--space-1) var(--space-2);
  border-radius: var(--radius-full);
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.stock-badge--in {
  background: var(--color-success-subtle);
  color: var(--color-success);
}

.stock-badge--out {
  background: var(--color-error-subtle);
  color: var(--color-error);
}

.stock-badge__dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: currentColor;
}
</style>
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
    expect(wrapper.find(".stock-badge--in").exists()).toBe(true);
  });

  it("shows 'Missing' when isInStock is false", () => {
    const wrapper = mount(IngredientStockBadge, { props: { isInStock: false } });
    expect(wrapper.text()).toContain("Missing");
    expect(wrapper.find(".stock-badge--out").exists()).toBe(true);
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
  <div class="recipe-detail-page">
    <div v-if="loading && !recipe">
      <SkeletonBlock height="40px" width="60%" class="mb-4" />
      <SkeletonBlock height="200px" />
    </div>
    <ErrorBanner v-else-if="error" :message="error" :show-retry="true" @retry="fetchRecipe" />

    <template v-else-if="recipe">
      <header class="recipe-detail__header">
        <NuxtLink to="/recipes" class="back-link">← Recipes</NuxtLink>
        <h1 class="recipe-detail__title">{{ recipe.name }}</h1>
        <div class="recipe-detail__tags">
          <span v-for="tag in recipe.tags" :key="tag" class="tag">{{ tag }}</span>
        </div>
        <p class="recipe-detail__meta">
          {{ recipe.prepMinutes + recipe.cookMinutes }} min total
          ({{ recipe.prepMinutes }} prep + {{ recipe.cookMinutes }} cook)
        </p>
      </header>

      <div class="recipe-detail__body">
        <section class="recipe-detail__section">
          <h2>Instructions</h2>
          <p class="recipe-detail__instructions">{{ recipe.instructions }}</p>
        </section>

        <section class="recipe-detail__section" v-if="enrichedIngredients.length > 0">
          <h2>Ingredients</h2>
          <ul class="ingredient-list">
            <li v-for="ing in enrichedIngredients" :key="ing.id" class="ingredient-item">
              <span class="ingredient-name">{{ ing.ingredientName }}</span>
              <span v-if="ing.quantity" class="ingredient-qty">{{ ing.quantity }}{{ ing.unit }}</span>
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

<style scoped>
.recipe-detail-page { max-width: 720px; margin: 0 auto; }

.back-link {
  display: inline-block;
  margin-bottom: var(--space-4);
  color: var(--color-accent);
  text-decoration: none;
  font-size: 14px;
}

.recipe-detail__header { margin-bottom: var(--space-8); }

.recipe-detail__title {
  font-family: var(--font-display);
  font-size: 32px;
  font-weight: 700;
  margin: 0 0 var(--space-3);
}

.recipe-detail__tags { display: flex; gap: var(--space-2); flex-wrap: wrap; margin-bottom: var(--space-3); }

.tag {
  background: var(--color-accent-subtle);
  color: var(--color-accent);
  border-radius: var(--radius-full);
  padding: var(--space-1) var(--space-3);
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
}

.recipe-detail__meta { color: var(--color-text-muted); font-size: 14px; margin: 0; }

.recipe-detail__body { display: flex; flex-direction: column; gap: var(--space-8); }

.recipe-detail__section h2 {
  font-family: var(--font-display);
  font-size: 20px;
  font-weight: 600;
  margin: 0 0 var(--space-4);
  padding-bottom: var(--space-2);
  border-bottom: 1px solid var(--color-border);
}

.recipe-detail__instructions {
  font-size: 15px;
  line-height: 1.7;
  color: var(--color-text-primary);
  margin: 0;
}

.ingredient-list { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: var(--space-3); }

.ingredient-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-3);
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  gap: var(--space-3);
}

.ingredient-name { font-weight: 500; }
.ingredient-qty { color: var(--color-text-muted); font-size: 13px; margin-right: auto; }

@media (max-width: 480px) {
  .recipe-detail__title { font-size: 24px; }
}
</style>
```

- [ ] **Step 4: Verify page renders**

Start dev server: `cd src/CookHomie.Web && npm run dev`
Visit `http://localhost:3000/recipes/r1`
Expected: Recipe name, tags, instructions, ingredient list with In Stock / Missing badges

- [ ] **Step 5: Run tests**

Run: `cd src/CookHomie.Web && npx vitest run tests/recipe-detail.spec.ts`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add src/CookHomie.Web/pages/recipes/[id].vue src/CookHomie.Web/composables/useRecipeDetail.ts src/CookHomie.Web/server/utils/recipeStore.ts
git commit -m "feat(web): implement recipe detail page with IngredientStockBadge and in-stock highlighting"
```
