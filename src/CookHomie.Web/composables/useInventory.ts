import type { AddInventoryItemPayload, InventoryItem } from "../types";

export const useInventory = () => {
  const polling = usePollingFetch<InventoryItem[]>("/api/inventory", { pollIntervalMs: 30000 });

  const items = polling.data;

  const loadInventory = async () => {
    await polling.refresh();
  };

  const addInventoryItem = async (payload: AddInventoryItemPayload): Promise<InventoryItem> => {
    polling.loading.value = true;
    polling.error.value = null;
    try {
      const created = await $fetch<InventoryItem>("/api/inventory", {
        method: "POST",
        body: payload,
      });
      // Append to local cache
      polling.data.value = [created, ...(polling.data.value ?? [])];
      return created;
    } catch (err) {
      polling.error.value = err instanceof Error ? err.message : "Failed to add inventory item";
      throw err;
    } finally {
      polling.loading.value = false;
    }
  };

  return {
    items,
    loading: polling.loading,
    error: polling.error,
    isStale: polling.isStale,
    loadInventory,
    startPolling: polling.start,
    stopPolling: polling.stop,
    refresh: polling.refresh,
    addInventoryItem,
  };
};
