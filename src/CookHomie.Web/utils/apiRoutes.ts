export const API_ROUTES = {
  // Dashboard routes
  DASHBOARD: {
    SUMMARY: "/api/dashboard/summary",
  },
  // Inventory routes
  INVENTORY: {
    BASE: "/api/inventory",
    DETAIL: (id: string) => `/api/inventory/${id}`,
  },
  // Recipe routes
  RECIPES: {
    BASE: "/api/recipes",
    DETAIL: (id: string) => `/api/recipes/${id}`,
    STOCK_CHECK: (id: string) => `/api/recipes/${id}/stock-check`,
  },
  // Shopping routes
  SHOPPING: {
    BASE: "/api/shopping",
    DETAIL: (id: string) => `/api/shopping/${id}`,
    BULK: "/api/shopping/bulk",
  },
} as const;
