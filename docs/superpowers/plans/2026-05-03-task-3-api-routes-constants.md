# Task 3: API Routes Constants File
**Branch:** `task/task-3-api-routes-constants`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Create: `src/CookHomie.Web/utils/apiRoutes.ts`
- Modify: `src/CookHomie.Web/composables/useDashboard.ts`
- Modify: `src/CookHomie.Web/composables/useInventory.ts`
- Modify: `src/CookHomie.Web/composables/useRecipes.ts`
- Modify: `src/CookHomie.Web/composables/useRecipeDetail.ts`
- Modify: `src/CookHomie.Web/composables/useShoppingList.ts`
- Modify: `src/CookHomie.Web/pages/recipes/[id].vue` (recipe detail calls `/api/recipes/${id}`)

## Steps

- [ ] **Step 1: Create `utils/apiRoutes.ts`**

Create `src/CookHomie.Web/utils/apiRoutes.ts`:

```typescript
export const API_ROUTES = {
  DASHBOARD: {
    SUMMARY: "/api/dashboard/summary",
  },
  INVENTORY: {
    LIST: "/api/inventory",
    DETAIL: (id: string) => `/api/inventory/${id}`,
  },
  RECIPES: {
    LIST: "/api/recipes",
    DETAIL: (id: string) => `/api/recipes/${id}`,
    MISSING: (id: string) => `/api/recipes/${id}/missing`,
  },
  SHOPPING: {
    LIST: "/api/shopping",
    DETAIL: (id: string) => `/api/shopping/${id}`,
    BULK: "/api/shopping/bulk",
  },
} as const;
```

- [ ] **Step 2: Migrate `useDashboard.ts`**

Replace endpoint string with `API_ROUTES.DASHBOARD.SUMMARY`:

```typescript
import { API_ROUTES } from "~/utils/apiRoutes";

const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<DashboardSummary>(
  API_ROUTES.DASHBOARD.SUMMARY,
  { pollIntervalMs: 30000 }
);
```

- [ ] **Step 3: Migrate `useInventory.ts`**

Replace `/api/inventory` with `API_ROUTES.INVENTORY.LIST`.

- [ ] **Step 4: Migrate `useRecipes.ts`**

Replace `/api/recipes` with `API_ROUTES.RECIPES.LIST`.

- [ ] **Step 5: Migrate `useRecipeDetail.ts`**

Replace the hardcoded `/api/recipes/${recipeId}` with `API_ROUTES.RECIPES.DETAIL(recipeId)`.

- [ ] **Step 6: Migrate `useShoppingList.ts`**

Replace `/api/shopping` with `API_ROUTES.SHOPPING.LIST`.

- [ ] **Step 7: Migrate `pages/recipes/[id].vue`**

The `$fetch` call inside `useRecipeDetail.ts` already uses the composable; no inline change needed there. However, verify the composable uses the route function.

- [ ] **Step 8: Commit**

```bash
git add src/CookHomie.Web/utils/apiRoutes.ts \
  src/CookHomie.Web/composables/useDashboard.ts \
  src/CookHomie.Web/composables/useInventory.ts \
  src/CookHomie.Web/composables/useRecipes.ts \
  src/CookHomie.Web/composables/useRecipeDetail.ts \
  src/CookHomie.Web/composables/useShoppingList.ts
git commit -m "feat(web): add API_ROUTES constants and migrate composables"
```
