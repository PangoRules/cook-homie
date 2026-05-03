// TEMP_MOCK: No C# API endpoint exists yet for GET /api/recipes/{id}.
import { getRecipeById } from "~/server/utils/recipeStore";
export default defineEventHandler((event) => {
  const id = getRouterParam(event, "id");
  const recipe = getRecipeById(id!);
  if (!recipe) {
    throw createError({ statusCode: 404, statusMessage: "Recipe not found" });
  }
  return recipe;
});
