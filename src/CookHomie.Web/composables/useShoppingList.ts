import type { ShoppingItem } from "../types";
import { computed } from "vue";

export const useShoppingList = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<ShoppingItem[]>(
    "/api/shopping",
    { pollIntervalMs: 30000 }
  );

  const items = computed<ShoppingItem[]>(() => data.value ?? []);

  // POST single item
  const addItem = async (payload: Omit<ShoppingItem, "id">): Promise<ShoppingItem> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<ShoppingItem>("/api/shopping", {
        method: "POST",
        body: payload,
      });
      data.value = [...(data.value ?? []), created];
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // POST bulk
  const addBulk = async (ingredientNames: string[]): Promise<ShoppingItem[]> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<ShoppingItem[]>("/api/shopping/bulk", {
        method: "POST",
        body: { ingredientNames },
      });
      data.value = [...(data.value ?? []), ...created];
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add items";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // PATCH (toggle bought, update qty/priority)
  const updateItem = async (id: string, updates: Partial<ShoppingItem>): Promise<ShoppingItem> => {
    loading.value = true;
    error.value = null;
    try {
      const updated = await $fetch<ShoppingItem>("/api/shopping/" + id, {
        method: "PATCH",
        body: updates,
      });
      data.value = (data.value ?? []).map((s) => s.id === id ? updated : s);
      return updated;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to update item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // DELETE
  const removeItem = async (id: string): Promise<void> => {
    loading.value = true;
    error.value = null;
    try {
      await $fetch("/api/shopping/" + id, { method: "DELETE" });
      data.value = (data.value ?? []).filter((s) => s.id !== id);
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to remove item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // Toggle bought
  const toggleBought = async (id: string) => {
    const item = items.value.find((s) => s.id === id);
    if (!item) return;
    await updateItem(id, { isBought: !item.isBought });
  };

  return {
    items,
    loading,
    error,
    isStale,
    loadList: refresh,
    startPolling: start,
    stopPolling: stop,
    refresh,
    addItem,
    addBulk,
    updateItem,
    removeItem,
    toggleBought,
  };
};
