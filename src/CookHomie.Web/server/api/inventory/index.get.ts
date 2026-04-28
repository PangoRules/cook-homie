import type { InventoryItem } from "../../../types";

export default defineEventHandler(async (_event): Promise<InventoryItem[]> => {
  const config = useRuntimeConfig();
  return await $fetch<InventoryItem[]>("/api/inventory", {
    baseURL: config.apiBaseUrl,
  });
});
