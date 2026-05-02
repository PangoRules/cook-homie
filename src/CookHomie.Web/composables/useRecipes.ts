import type { Recipe } from "~/types";

export const useRecipes = () => {
  const recipes = useState<Recipe[]>("recipes", () => []);
  const loading = useState("recipes-loading", () => true);
  const error = useState<string | null>("recipes-error", () => null);
  const isStale = useState("recipes-stale", () => false);

  let intervalId: ReturnType<typeof setInterval> | null = null;

  const fetchRecipes = async () => {
    loading.value = true;
    error.value = null;
    try {
      recipes.value = await $fetch<Recipe[]>("/api/recipes");
      isStale.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load recipes";
      isStale.value = recipes.value.length > 0;
    } finally {
      loading.value = false;
    }
  };

  const start = () => {
    fetchRecipes();
    intervalId = setInterval(fetchRecipes, 30000);
  };
  const stop = () => {
    if (intervalId) {
      clearInterval(intervalId);
      intervalId = null;
    }
  };
  const refresh = () => fetchRecipes();

  return { recipes, loading, error, isStale, start, stop, refresh };
};

