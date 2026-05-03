// TEMP_MOCK: No C# API endpoint exists yet for POST /api/recipes.
import { addRecipe } from "~/server/utils/recipeStore";
export default defineEventHandler(async (event) => {
  const body = await readBody(event);
  if (!body?.name || !body?.instructions) {
    throw createError({ statusCode: 400, statusMessage: "name and instructions are required" });
  }
  return addRecipe(body);
});
