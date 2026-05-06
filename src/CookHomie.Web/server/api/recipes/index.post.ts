import type { Recipe } from "~/types";

export default defineEventHandler(async (event): Promise<Recipe> => {
  const config = useRuntimeConfig();
  const body = await readBody(event);
  return await $fetch<Recipe>("/api/recipes", {
    baseURL: config.apiBaseUrl,
    method: "POST",
    body,
  });
});
