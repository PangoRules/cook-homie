import type { InventoryItem } from "../../../types";

export default defineEventHandler(async (event): Promise<InventoryItem> => {
  const config = useRuntimeConfig();
  const body = await readBody(event);
  return await $fetch<InventoryItem>("/api/inventory", {
    baseURL: config.apiBaseUrl,
    method: "POST",
    body,
  });
});
