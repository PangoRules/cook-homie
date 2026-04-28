export default defineNuxtConfig({
  modules: ["@nuxt/eslint"],
  compatibilityDate: "2025-01-01",
  runtimeConfig: {
    apiBaseUrl: process.env.NUXT_API_BASE_URL ?? "http://localhost:5000",
    public: {
      appEnv: process.env.NODE_ENV ?? "development",
    },
  },
  css: ["@/assets/css/tokens.css"],
});
