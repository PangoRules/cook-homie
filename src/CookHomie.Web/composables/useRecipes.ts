import type { Recipe, AddRecipePayload } from "../types";

export const useRecipes = () => {
  const recipes = useState<Recipe[]>("recipes", () => []);
  const loading = useState<boolean>("recipes-loading", () => false);
  const error = useState<string | null>("recipes-error", () => null);
  const isStale = useState<boolean>("recipes-stale", () => false);

  const fetchRecipes = async () => {
    loading.value = true;
    error.value = null;
    try {
      const result = await $fetch<Recipe[]>("/api/recipes");
      recipes.value = result;
      isStale.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load recipes";
      isStale.value = recipes.value.length > 0;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  let intervalId: ReturnType<typeof setInterval> | null = null;

  const startPolling = () => {
    fetchRecipes();
    intervalId = setInterval(fetchRecipes, 30000);
  };

  const stopPolling = () => {
    if (intervalId !== null) {
      clearInterval(intervalId);
      intervalId = null;
    }
  };

  const refresh = async () => {
    await fetchRecipes();
  };

  const loadRecipes = async () => {
    await fetchRecipes();
  };

  const addRecipe = async (recipe: AddRecipePayload): Promise<Recipe> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<Recipe>("/api/recipes", { method: "POST", body: recipe });
      recipes.value.unshift(created);
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add recipe";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  return {
    recipes,
    loading,
    error,
    isStale,
    loadRecipes,
    startPolling,
    stopPolling,
    refresh,
    addRecipe,
  };
};
