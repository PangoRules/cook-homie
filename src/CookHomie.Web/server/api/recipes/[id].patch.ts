// TEMP_MOCK: No C# Recipes API exists yet.
// For MVP, updates go to in-memory mock store.
// In production: forward to http://localhost:5000/api/recipes/${id} with PATCH.
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

  if (!updated) {
    throw createError({ statusCode: 404, statusMessage: "Recipe not found" });
  }

  return updated;
});