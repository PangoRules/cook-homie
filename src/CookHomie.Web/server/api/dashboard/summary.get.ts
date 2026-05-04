import type { DashboardSummary } from "~/types";

// TEMP_MOCK: No C# API endpoint exists yet for GET /api/dashboard/summary.
// Replace this route with a proxy when backend DashboardController exists.
export default defineEventHandler(
  (): DashboardSummary => ({
    expiringCount: 3,
    recipeMatchCount: 2,
    shoppingCount: 4,
    totalItems: 15,
    expiringItems: [
      { id: "inv-1", name: "Milk", expiresAt: "2026-05-03", location: "Fridge" },
      { id: "inv-2", name: "Spinach", expiresAt: "2026-05-04", location: "Fridge" },
      { id: "inv-3", name: "Yogurt", expiresAt: "2026-05-05", location: "Fridge" },
    ],
    recipeIdeas: [
      { id: "r1", name: "Classic Pancakes", matchedCount: 2, missingCount: 1 },
      { id: "r3", name: "Avocado Toast", matchedCount: 2, missingCount: 0 },
    ],
  })
);
