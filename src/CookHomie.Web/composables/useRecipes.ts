import type { Recipe } from "~/types";
import { computed } from "vue";
import { API_ROUTES } from "~/utils/apiRoutes";

export const useRecipes = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<Recipe[]>(
    API_ROUTES.RECIPES,
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
