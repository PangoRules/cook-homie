export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig();
  return await $fetch("/api/inventory", {
    baseURL: config.apiBaseUrl
  });
});
