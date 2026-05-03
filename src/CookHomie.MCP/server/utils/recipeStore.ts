/**
 * Recipe store implementation that properly persists ingredients with IDs
 */
import { ApiClient } from "../../api_client";

const client = new ApiClient();

export interface RecipeIngredient {
  id?: string;
  name: string;
  quantity?: number;
  unit?: string;
}

export interface Recipe {
  id?: string;
  name: string;
  ingredients: RecipeIngredient[];
  instructions: string;
  prepTime?: number;
  cookTime?: number;
  servings?: number;
  tags?: string[];
}

export class RecipeStore {
  /**
   * Add a recipe with properly mapped ingredient IDs
   */
  static async addRecipe(recipe: Recipe): Promise<Recipe> {
    // Map ingredients to ensure they have proper IDs
    const ingredientsWithIds = recipe.ingredients.map(ingredient => {
      // If ingredient already has an ID, keep it
      if (ingredient.id) {
        return ingredient;
      }
      // For new ingredients, we don't need to create IDs in the recipe store
      // The API will handle ingredient creation and ID assignment
      return {
        name: ingredient.name,
        quantity: ingredient.quantity,
        unit: ingredient.unit
      };
    });

    // Create the recipe with ingredients (API handles the rest)
    const newRecipe = await client.addRecipe({
      ...recipe,
      ingredients: ingredientsWithIds
    });

    return newRecipe;
  }

  /**
   * Get all recipes
   */
  static async getRecipes(query?: string): Promise<Recipe[]> {
    return await client.getRecipes(query);
  }
}