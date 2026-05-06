import type { RecipeStockCheck } from "~/types";

export default defineEventHandler(async (event): Promise<RecipeStockCheck> => {
  const config = useRuntimeConfig();
  const id = getRouterParam(event, "id")!;
  return await $fetch<RecipeStockCheck>(`/api/recipes/${id}/stock-check`, {
    baseURL: config.apiBaseUrl,
  });
});
