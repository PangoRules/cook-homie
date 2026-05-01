// TEMP_MOCK: No C# API endpoint exists yet for GET /api/recipes.
import { getRecipes } from "~/server/utils/recipeStore";
export default defineEventHandler(() => getRecipes());