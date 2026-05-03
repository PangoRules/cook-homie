export const API_ROUTES = {
  // Inventory routes
  INVENTORY: '/api/inventory',
  INVENTORY_ITEM: (id: string) => `/api/inventory/${id}`,
  INVENTORY_BULK: '/api/inventory/bulk',

  // Recipe routes
  RECIPES: '/api/recipes',
  RECIPE: (id: string) => `/api/recipes/${id}`,
  RECIPE_MISSING: (id: string) => `/api/recipes/${id}/missing`,

  // Shopping routes
  SHOPPING: '/api/shopping',
  SHOPPING_ITEM: (id: string) => `/api/shopping/${id}`,
  SHOPPING_BULK: '/api/shopping/bulk',

  // Dashboard routes
  DASHBOARD: {
    SUMMARY: '/api/dashboard/summary',
  },
} as const;