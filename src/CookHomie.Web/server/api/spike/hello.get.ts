export default defineEventHandler(async () => {
  const config = useRuntimeConfig();

  return await $fetch("/spike/hello", {
    baseURL: config.apiBaseUrl
  });
});
