import type { Recipe } from "~/types";
import { API_ROUTES } from "~/utils/apiRoutes";

export default defineEventHandler(async (_event): Promise<Recipe[]> => {
  const config = useRuntimeConfig();
  return await $fetch<Recipe[]>(API_ROUTES.RECIPES.BASE, {
    baseURL: config.apiBaseUrl,
  });
});
