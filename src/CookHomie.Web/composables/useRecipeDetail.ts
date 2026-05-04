import type { Recipe, RecipeIngredient } from "~/types";
import { matchIngredientStock } from "~/utils/ingredientStock";
import { useInventory } from "./useInventory";
import { API_ROUTES } from "~/utils/apiRoutes";

export const useRecipeDetail = (recipeId: string) => {
  const recipe = useState<Recipe | null>(`recipe-${recipeId}`, () => null);
  const loading = useState(`recipe-loading-${recipeId}`, () => false);
  const error = useState<string | null>(`recipe-error-${recipeId}`, () => null);

  // Derive inventory names from the shared inventory state
  const { items: inventoryItems, loadInventory } = useInventory();
  const inventoryNames = computed(() =>
    inventoryItems.value.map(item => item.name)
  );

  const fetchRecipe = async () => {
    loading.value = true;
    error.value = null;
    try {
      // Ensure inventory is loaded first
      await loadInventory();
      recipe.value = await $fetch<Recipe>(API_ROUTES.RECIPES.DETAIL(recipeId));
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

const updateRecipe = async (updates: Partial<Omit<Recipe, "id">>) => {
  loading.value = true;
  error.value = null;
  try {
    const updated = await $fetch<Recipe>(API_ROUTES.RECIPES.DETAIL(recipeId), {
      method: "PATCH",
      body: updates,
    });
    recipe.value = updated;
    return updated;
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Failed to update recipe";
    throw err;
  } finally {
    loading.value = false;
  }
};

const start = () => fetchRecipe();
const stop = () => {};

return { recipe, loading, error, enrichedIngredients, fetchRecipe, updateRecipe, start, stop };
};