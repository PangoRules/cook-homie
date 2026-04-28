import { getMissingIngredients } from "~/server/utils/recipeStore";
export default defineEventHandler((event) => {
  const id = getRouterParam(event, "id");
  return getMissingIngredients(id!, []);
});