# Task 7: Shared Paginated Ingredient Table + AddRecipeModal Validation
**Branch:** `task/task-7-shared-ingredient-table`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Create: `src/CookHomie.Web/components/shared/IngredientTable.vue`
- Modify: `src/CookHomie.Web/components/recipes/AddRecipeModal.vue`
- Modify: `src/CookHomie.Web/pages/recipes/[id].vue` (replace local list with component — Task 8 in parallel, can do now)

## Steps

- [ ] **Step 1: Create `components/shared/IngredientTable.vue`**

The component accepts an array of `RecipeIngredient` and supports two modes via props.

**Props:**
```typescript
interface Props {
  ingredients: RecipeIngredient[];
  mode?: "readonly" | "editable";
  pageSize?: number;
}
```

**Pagination:** computed page slice using `currentPage` ref and `pageSize` prop (default 5). Prev/Next buttons. Shows "Page X of Y".

**Readonly mode (for recipe detail):** renders each row as: name | qty+unit | optional badge | stock badge. No remove button.

**Editable mode (for AddRecipeModal):** same columns + per-row remove button + optional toggle. Emits `update:ingredients` for add/remove/change operations.

**Slots:**
- `#stock-badge` — allows parent to inject `IngredientStockBadge` component (passed from consumer)
- `#actions` — extra action column for editable mode

**Template structure:**
```vue
<template>
  <div class="flex flex-col gap-2">
    <table class="w-full text-sm">
      <thead>
        <tr class="border-b border-border">
          <th class="text-left text-text-muted font-medium">Ingredient</th>
          <th class="text-right text-text-muted font-medium w-[80px]">Qty</th>
          <th class="text-right text-text-muted font-medium w-[60px]">Unit</th>
          <th class="text-center text-text-muted font-medium w-[60px]">Optional</th>
          <th v-if="mode === 'readonly'" class="text-center text-text-muted font-medium w-[80px]">Status</th>
          <th v-if="mode === 'editable'" class="w-[60px]" />
        </tr>
      </thead>
      <tbody>
        <tr v-for="(ing, i) in paginatedIngredients" :key="i" class="border-b border-border">
          <td class="py-2">{{ ing.ingredientName }}</td>
          <td class="py-2 text-right">{{ ing.quantity }}</td>
          <td class="py-2 text-right">{{ ing.unit }}</td>
          <td class="py-2 text-center">{{ ing.isOptional ? "Yes" : "—" }}</td>
          <td v-if="mode === 'readonly' && $slots['stock-badge']" class="py-2 text-center">
            <slot name="stock-badge" :ing="ing" />
          </td>
          <td v-if="mode === 'editable'" class="py-2 text-right">
            <button type="button" class="btn btn-secondary text-xs" @click="$emit('remove', i)">Remove</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Pagination controls -->
    <div class="flex items-center justify-between">
      <span class="text-xs text-text-muted">Page {{ currentPage }} of {{ totalPages }}</span>
      <div class="flex gap-2">
        <SharedButton size="small" variant="secondary" :disabled="currentPage === 1" @click="currentPage--">
          Prev
        </SharedButton>
        <SharedButton size="small" variant="secondary" :disabled="currentPage === totalPages" @click="currentPage++">
          Next
        </SharedButton>
      </div>
    </div>
  </div>
</template>
```

- [ ] **Step 2: Integrate into `AddRecipeModal.vue`**

Replace the inline ingredient grid (lines 26–67) with:

```vue
<SharedFormField label="Ingredients" :error="errors.ingredients" required>
  <IngredientTable
    v-model:ingredients="form.ingredients"
    mode="editable"
    :page-size="5"
  />
  <button type="button" class="btn btn-secondary w-fit mt-2" @click="addIngredient">
    + Add Ingredient
  </button>
</SharedFormField>
```

Note: `IngredientTable` in editable mode handles removal via `v-model:ingredients` update. The modal's `addIngredient` adds a new empty row via `form.ingredients.push(emptyIngredient())`.

Also update validation in `AddRecipeModal.vue`: add `prepMinutes`/`cookMinutes` validation requiring >= 1:

```typescript
if (form.prepMinutes < 1) { errors.prepMinutes = "Must be at least 1 minute"; isValid = false; }
if (form.cookMinutes < 1) { errors.cookMinutes = "Must be at least 1 minute"; isValid = false; }
```

Add `errors.prepMinutes` and `errors.cookMinutes` to the `errors` reactive object.

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/components/shared/IngredientTable.vue \
  src/CookHomie.Web/components/recipes/AddRecipeModal.vue
git commit -m "feat(web): add shared IngredientTable with pagination + time validation"
```
