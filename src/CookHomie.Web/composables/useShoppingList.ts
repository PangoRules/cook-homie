import type { ShoppingItem } from "../types";

export const useShoppingList = () => {
  const items = useState<ShoppingItem[]>("shopping-items", () => []);
  const loading = useState<boolean>("shopping-loading", () => false);
  const error = useState<string | null>("shopping-error", () => null);
  const isStale = useState<boolean>("shopping-stale", () => false);

  const fetchItems = async () => {
    loading.value = true;
    error.value = null;
    try {
      const result = await $fetch<ShoppingItem[]>("/api/shopping");
      items.value = result;
      isStale.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load shopping list";
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

  const refresh = async () => {
    await fetchItems();
  };

  const loadList = async () => {
    await fetchItems();
  };

  const addToList = async (ingredientNames: string[]): Promise<ShoppingItem[]> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<ShoppingItem[]>("/api/shopping", {
        method: "POST",
        body: { ingredientNames },
      });
      items.value.push(...created);
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
    startPolling,
    stopPolling,
    refresh,
    addToList,
  };
};
