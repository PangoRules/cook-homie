export default defineNuxtConfig({
  compatibilityDate: "2025-01-01",
  runtimeConfig: {
    apiBaseUrl: process.env.COOKHOMIE_API_BASE_URL ?? "http://localhost:5000"
  }
});
