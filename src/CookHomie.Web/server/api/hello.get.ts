export default defineEventHandler(async (_event): Promise<{ message: string }> => {
  const config = useRuntimeConfig();

  return await $fetch<{ message: string }>("/api/hello", {
    baseURL: config.apiBaseUrl,
    headers: {
      Accept: "application/json",
    },
  });
});
