export default defineEventHandler(async () => {
  const config = useRuntimeConfig();

  try {
    return await $fetch("/spike/hello", {
      baseURL: config.apiBaseUrl
    });
  } catch {
    throw createError({
      statusCode: 502,
      statusMessage: "Bad Gateway",
      message: "Unable to reach upstream API"
    });
  }
});
