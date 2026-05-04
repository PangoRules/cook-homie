import type { Recipe } from "~/types";

export default defineEventHandler(async (event): Promise<Recipe> => {
  const config = useRuntimeConfig();
  const id = getRouterParam(event, "id")!;
  return await $fetch<Recipe>(`/api/recipes/${id}`, {
    baseURL: config.apiBaseUrl,
  });
});
