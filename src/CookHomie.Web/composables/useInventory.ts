import type { AddInventoryItemPayload, InventoryItem } from "../types";

export const useInventory = () => {
  const items = useState<InventoryItem[]>("inventory-items", () => []);
  const loading = useState<boolean>("inventory-loading", () => false);
  const error = useState<string | null>("inventory-error", () => null);
  const isStale = useState<boolean>("inventory-stale", () => false);

  const fetchItems = async () => {
    loading.value = true;
    error.value = null;
    try {
      const result = await $fetch<InventoryItem[]>("/api/inventory");
      items.value = result;
      isStale.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load inventory";
      isStale.value = items.value.length > 0;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  let intervalId: ReturnType<typeof setInterval> | null = null;

  const startPolling = () => {
    fetchItems();
    intervalId = setInterval(fetchItems, 30000);
  };

  const stopPolling = () => {
    if (intervalId !== null) {
      clearInterval(intervalId);
      intervalId = null;
    }
  };

  const refresh = async () => { await fetchItems(); };

  const loadInventory = async () => { await fetchItems(); };

  const addInventoryItem = async (payload: AddInventoryItemPayload): Promise<InventoryItem> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<InventoryItem>("/api/inventory", { method: "POST", body: payload });
      items.value.unshift(created);
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add inventory item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  return {
    items, loading, error, isStale,
    loadInventory, startPolling, stopPolling, refresh, addInventoryItem
  };
};
