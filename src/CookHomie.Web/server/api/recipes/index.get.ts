import type { Recipe } from "~/types";

export default defineEventHandler(async (_event): Promise<Recipe[]> => {
  const config = useRuntimeConfig();
  return await $fetch<Recipe[]>("/api/recipes", {
    baseURL: config.apiBaseUrl,
  });
});
