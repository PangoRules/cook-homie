// TEMP_MOCK: No C# API endpoint exists yet for GET /api/dashboard/summary.
// Replace this route with a proxy when backend DashboardController exists.
export default defineEventHandler(() => ({
  expiringCount: 3,
  recipeMatchCount: 7,
  shoppingCount: 4,
  totalItems: 15,
  upcomingExpirations: ["2024-05-15", "2024-05-20", "2024-05-25"],
  recommendedRecipes: ["Spaghetti", "Chicken Salad", "Vegetable Stir Fry"]
}));