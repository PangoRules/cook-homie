import { getRecipes } from "~/server/utils/recipeStore";
export default defineEventHandler(() => getRecipes());