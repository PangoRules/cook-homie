import type { Recipe } from "~/types";

export default defineEventHandler(async (event): Promise<Recipe> => {
  const config = useRuntimeConfig();
  const id = getRouterParam(event, "id")!;
  const body = await readBody(event);
  return await $fetch<Recipe>(`/api/recipes/${id}`, {
    baseURL: config.apiBaseUrl,
    method: "PATCH",
    body,
  });
});
