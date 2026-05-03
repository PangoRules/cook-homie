import type { Recipe } from "~/types";
import { computed } from "vue";

export const useRecipes = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<Recipe[]>(
    "/api/recipes",
    { pollIntervalMs: 30000 }
  );

  const recipes = computed<Recipe[]>(() => data.value ?? []);

  return {
    recipes,
    loading, 
    error, 
    isStale, 
    start, 
    stop, 
    refresh 
  };
};
