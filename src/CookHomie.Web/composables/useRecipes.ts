import type { Recipe } from "~/types";

export const useRecipes = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<Recipe[]>(
    "/api/recipes",
    { pollIntervalMs: 30000 }
  );

  return { 
    recipes: data, 
    loading, 
    error, 
    isStale, 
    start, 
    stop, 
    refresh 
  };
};

