import type { ShoppingItem } from "../types";
import { computed } from "vue";

export const useShoppingList = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<ShoppingItem[]>(
    "/api/shopping",
    { pollIntervalMs: 30000 }
  );

  const items = computed<ShoppingItem[]>(() => data.value ?? []);

  const loadList = async () => {
    await refresh();
  };

  const addToList = async (ingredientNames: string[]): Promise<ShoppingItem[]> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<ShoppingItem[]>("/api/shopping", {
        method: "POST",
        body: { ingredientNames },
      });
      data.value = [...(data.value ?? []), ...created];
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add items to shopping list";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  return {
    items,
    loading,
    error,
    isStale,
    loadList,
    startPolling: start,
    stopPolling: stop,
    refresh,
    addToList,
  };
};
