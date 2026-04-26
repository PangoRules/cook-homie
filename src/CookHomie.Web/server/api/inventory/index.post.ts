export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig();
  const body = await readBody(event);

  return await $fetch("/api/inventory", {
    baseURL: config.apiBaseUrl,
    method: "POST",
    body
  });
});
