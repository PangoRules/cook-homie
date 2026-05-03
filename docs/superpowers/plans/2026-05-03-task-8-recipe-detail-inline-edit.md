# Task 8: Recipe Detail — Shared Table, Shopping Actions, Inline Edit
**Branch:** `task/task-8-recipe-detail-inline-edit`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Modify: `src/CookHomie.Web/pages/recipes/[id].vue`
- Create: `src/CookHomie.Web/server/api/recipes/[id].patch.ts` (Nuxt server route for PATCH)
- Modify: `src/CookHomie.Web/composables/useRecipeDetail.ts`

## Dependencies
- Task 7 (shared IngredientTable) must be completed first.
- Task 9 (shopping API) should be available for per-row "Add to shopping list" deduplication.

## Steps

- [ ] **Step 1: Create `server/api/recipes/[id].patch.ts`**

Create `src/CookHomie.Web/server/api/recipes/[id].patch.ts`. This is a Nuxt server proxy that forwards to the C# API (or falls back to the mock store for MVP):

```typescript
import { getRecipeById, updateRecipe } from "~/server/utils/recipeStore";

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, "id");
  const body = await readBody(event);

  const recipe = getRecipeById(id!);
  if (!recipe) {
    throw createError({ statusCode: 404, statusMessage: "Recipe not found" });
  }

  // Partial update — merge with existing
  const updated = updateRecipe(id!, {
    name: body.name ?? recipe.name,
    instructions: body.instructions ?? recipe.instructions,
    prepMinutes: body.prepMinutes ?? recipe.prepMinutes,
    cookMinutes: body.cookMinutes ?? recipe.cookMinutes,
    tags: body.tags ?? recipe.tags,
    ingredients: body.ingredients ?? recipe.ingredients,
  });

  return updated;
});
```

Note: In production this calls `http://localhost:5000/api/recipes/${id}` with PATCH. For MVP, the mock store update is sufficient.

- [ ] **Step 2: Update `useRecipeDetail.ts` — add edit mode + patch support**

Add `updateRecipe` function:

```typescript
const updateRecipe = async (updates: Partial<Omit<Recipe, "id">>) => {
  loading.value = true;
  error.value = null;
  try {
    const updated = await $fetch<Recipe>(`/api/recipes/${recipeId}`, {
      method: "PATCH",
      body: updates,
    });
    recipe.value = updated;
    return updated;
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Failed to update recipe";
    throw err;
  } finally {
    loading.value = false;
  }
};
```

Expose `updateRecipe` from the composable.

- [ ] **Step 3: Upgrade `pages/recipes/[id].vue` — inline edit + shared table**

Refactor the template to use section-level edit mode toggles. The page has three editable sections: **Meta** (name, prep/cook time, tags), **Instructions**, **Ingredients**.

**Edit mode state:**
```typescript
const editMode = ref<"none" | "meta" | "instructions" | "ingredients">("none");
const editBuffer = ref<Partial<Recipe>>({});

const startEditMeta = () => { editBuffer.value = { ...recipe.value }; editMode.value = "meta"; };
const startEditInstructions = () => { editBuffer.value = { instructions: recipe.value.instructions }; editMode.value = "instructions"; };
const startEditIngredients = () => { editMode.value = "ingredients"; };

const cancelEdit = () => { editMode.value = "none"; };
const saveEdit = async () => {
  await updateRecipe(editBuffer.value);
  editMode.value = "none";
  useToast().pushSuccess("Recipe updated");
};
```

**Meta section — display vs edit:**
```vue
<section>
  <div class="flex items-center justify-between mb-3">
    <h2 class="font-display text-[20px] font-semibold">Recipe Info</h2>
    <button v-if="editMode !== 'meta'" class="btn btn-secondary text-xs" @click="startEditMeta">Edit</button>
  </div>
  <div v-if="editMode !== 'meta'">
    <!-- display: name, tags, time -->
  </div>
  <div v-else>
    <!-- edit form with inputs -->
    <SharedButton variant="secondary" @click="cancelEdit">Cancel</SharedButton>
    <SharedButton variant="primary" @click="saveEdit">Save</SharedButton>
  </div>
</section>
```

Apply the same pattern for **Instructions** and **Ingredients** sections.

**Ingredients section — use shared table:**
```vue
<IngredientTable
  :ingredients="editMode === 'ingredients' ? editIngredients : enrichedIngredients"
  mode="readonly"
>
  <template #stock-badge="{ ing }">
    <RecipesDetailIngredientStockBadge :is-in-stock="ing.isInStock ?? false" />
    <button
      v-if="!(ing.isInStock ?? false)"
      class="ml-2 text-xs text-accent hover:underline"
      @click="addMissingToShoppingList(ing.ingredientName)"
    >
      + Add to list
    </button>
  </template>
</IngredientTable>
```

- [ ] **Step 4: Add `addMissingToShoppingList` per-row function**

```typescript
const addMissingToShoppingList = async (ingredientName: string) => {
  const { addBulk, items } = useShoppingList();
  const alreadyOnList = items.value.some(
    (s) => s.name.toLowerCase() === ingredientName.toLowerCase() && !s.isBought
  );
  if (alreadyOnList) {
    useToast().pushSuccess("Already on shopping list");
    return;
  }
  try {
    await addBulk([ingredientName]);
    useToast().pushSuccess(`"${ingredientName}" added to shopping list`);
  } catch {
    useToast().pushError("Failed to add to shopping list");
  }
};
```

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/pages/recipes/[id].vue \
  src/CookHomie.Web/composables/useRecipeDetail.ts \
  src/CookHomie.Web/server/api/recipes/[id].patch.ts
git commit -m "feat(web): recipe detail inline edit + shared table + shopping row actions"
```
