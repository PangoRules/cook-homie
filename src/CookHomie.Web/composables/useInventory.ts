import type { AddInventoryItemPayload, InventoryItem } from "../types";

export const useInventory = () => {
  const items = useState<InventoryItem[]>("inventory-items", () => []);
  const loading = useState<boolean>("inventory-loading", () => false);
  const error = useState<string | null>("inventory-error", () => null);

  const loadInventory = async () => {
    loading.value = true;
    error.value = null;

    try {
      items.value = await $fetch<InventoryItem[]>("/api/inventory");
      return items.value;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load inventory";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const addInventoryItem = async (payload: AddInventoryItemPayload): Promise<InventoryItem> => {
    loading.value = true;
    error.value = null;

    try {
      const created = await $fetch<InventoryItem>("/api/inventory", {
        method: "POST",
        body: payload
      });
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
    items,
    loading,
    error,
    loadInventory,
    addInventoryItem
  };
};
