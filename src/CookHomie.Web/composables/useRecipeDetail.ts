import type { Recipe, RecipeIngredient } from "~/types";
import { matchIngredientStock } from "~/utils/ingredientStock";
import { useInventory } from "./useInventory";

export const useRecipeDetail = (recipeId: string) => {
  const recipe = useState<Recipe | null>(`recipe-${recipeId}`, () => null);
  const loading = useState(`recipe-loading-${recipeId}`, () => false);
  const error = useState<string | null>(`recipe-error-${recipeId}`, () => null);

  // Derive inventory names from the shared inventory state
  const { items: inventoryItems } = useInventory();
  const inventoryNames = computed(() =>
    inventoryItems.value.map(item => item.name)
  );

  const fetchRecipe = async () => {
    loading.value = true;
    error.value = null;
    try {
      recipe.value = await $fetch<Recipe>(`/api/recipes/${recipeId}`);
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load recipe";
    } finally {
      loading.value = false;
    }
  };

  const enrichedIngredients = computed<RecipeIngredient[]>(() => {
    if (!recipe.value) return [];
    return recipe.value.ingredients.map(ing => {
      // Add isInStock property to the ingredient
      const enrichedIng = { ...ing };
      enrichedIng.isInStock = matchIngredientStock(ing.ingredientName, inventoryNames.value);
      return enrichedIng;
    });
  });

  const start = () => fetchRecipe();
  const stop = () => {};

  return { recipe, loading, error, enrichedIngredients, fetchRecipe, start, stop };
};