export default defineEventHandler(async (event): Promise<string[]> => {
  const config = useRuntimeConfig();
  const id = getRouterParam(event, "id")!;
  return await $fetch<string[]>(`/api/recipes/${id}/missing`, {
    baseURL: config.apiBaseUrl,
  });
});
