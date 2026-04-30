import tailwindcss from "@tailwindcss/vite";

export default defineNuxtConfig({
  modules: ["@nuxt/eslint"],
  compatibilityDate: "2025-01-01",
  vite: {
    plugins: [tailwindcss()],
  },
  runtimeConfig: {
    apiBaseUrl: process.env.NUXT_API_BASE_URL ?? "http://localhost:5000",
    public: {
      appEnv: process.env.NODE_ENV ?? "development",
    },
  },
  css: ["~/assets/css/main.css"],
});
