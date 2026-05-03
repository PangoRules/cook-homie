export const API_ROUTES = {
  // Dashboard routes
  DASHBOARD: {
    SUMMARY: '/api/dashboard/summary',
  },
  // Inventory routes
  INVENTORY: {
    LIST: '/api/inventory',
    DETAIL: (id: string) => `/api/inventory/${id}`,
  },
  // Recipe routes
  RECIPES: {
    LIST: '/api/recipes',
    DETAIL: (id: string) => `/api/recipes/${id}`,
    MISSING: (id: string) => `/api/recipes/${id}/missing`,
  },
  // Shopping routes
  SHOPPING: {
    LIST: '/api/shopping',
    DETAIL: (id: string) => `/api/shopping/${id}`,
    BULK: '/api/shopping/bulk',
  },
} as const;