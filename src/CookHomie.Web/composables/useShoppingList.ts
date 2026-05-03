import type { ShoppingItem } from "../types";

export const useShoppingList = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<ShoppingItem[]>(
    "/api/shopping",
    { pollIntervalMs: 30000 }
  );

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
      // Note: usePollingFetch handles the refresh automatically, but we should add to local state for immediate UX
      // This is a design consideration that would require more sophisticated state management
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add items to shopping list";
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
    loadList,
    startPolling: start,
    stopPolling: stop,
    refresh,
    addToList,
  };
};
