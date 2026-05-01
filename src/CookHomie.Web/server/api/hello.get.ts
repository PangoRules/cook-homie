export default defineEventHandler(async (_event) => {
  const config = useRuntimeConfig();

  return await $fetch("/api/hello", {
    baseURL: config.apiBaseUrl,
    method: "GET",
    headers: {
      Accept: "application/json",
    },
  });
});
