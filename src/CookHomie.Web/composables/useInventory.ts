import type { AddInventoryItemPayload, InventoryItem } from "../types";

export const useInventory = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<InventoryItem[]>(
    "/api/inventory",
    { pollIntervalMs: 30000 }
  );

  const loadInventory = async () => {
    await refresh();
  };

  const addInventoryItem = async (payload: AddInventoryItemPayload): Promise<InventoryItem> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<InventoryItem>("/api/inventory", {
        method: "POST",
        body: payload,
      });
      // Note: usePollingFetch handles the refresh automatically, but we should add to local state for immediate UX
      // This is a design consideration that would require more sophisticated state management
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add inventory item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  return {
    items: data,
    loading,
    error,
    isStale,
    loadInventory,
    startPolling: start,
    stopPolling: stop,
    refresh,
    addInventoryItem,
  };
};
