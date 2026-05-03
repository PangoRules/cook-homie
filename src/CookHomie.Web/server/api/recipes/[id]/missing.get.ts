// TEMP_MOCK: No C# API endpoint exists yet for GET /api/recipes/{id}/missing.
import { getMissingIngredients } from "~/server/utils/recipeStore";
export default defineEventHandler((event) => {
  const id = getRouterParam(event, "id");
  return getMissingIngredients(id!, []);
});
