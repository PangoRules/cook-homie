// TEMP_MOCK: No C# Recipes API exists yet.
// This in-memory store must be deleted once backend RecipesController ships.
import type { Recipe, RecipeIngredient } from "~/types";

const recipes: Recipe[] = [
  {
    id: "r1",
    name: "Classic Pancakes",
    instructions: "Mix flour, eggs, milk. Cook on griddle.",
    prepMinutes: 5,
    cookMinutes: 10,
    tags: ["breakfast", "quick"],
    ingredients: [
      { id: "i1", recipeId: "r1", ingredientName: "flour",     quantity: 200, unit: "g",   isOptional: false },
      { id: "i2", recipeId: "r1", ingredientName: "eggs",      quantity: 2,   unit: "pcs", isOptional: false },
      { id: "i3", recipeId: "r1", ingredientName: "milk",      quantity: 250, unit: "ml",  isOptional: false },
      { id: "i4", recipeId: "r1", ingredientName: "butter",    quantity: 30,  unit: "g",   isOptional: true  },
    ] as RecipeIngredient[],
  },
  {
    id: "r2",
    name: "Tomato Pasta",
    instructions: "Boil pasta. Sauté garlic, add tomatoes, toss.",
    prepMinutes: 5,
    cookMinutes: 15,
    tags: ["pasta", "vegetarian"],
    ingredients: [
      { id: "i5", recipeId: "r2", ingredientName: "pasta",     quantity: 200, unit: "g",   isOptional: false },
      { id: "i6", recipeId: "r2", ingredientName: "tomatoes",  quantity: 400, unit: "g",   isOptional: false },
      { id: "i7", recipeId: "r2", ingredientName: "garlic",    quantity: 3,   unit: "cloves", isOptional: false },
    ] as RecipeIngredient[],
  },
  {
    id: "r3",
    name: "Avocado Toast",
    instructions: "Toast bread. Mash avocado with salt and lemon. Spread.",
    prepMinutes: 3,
    cookMinutes: 2,
    tags: ["breakfast", "quick", "vegetarian"],
    ingredients: [
      { id: "i8", recipeId: "r3", ingredientName: "bread",     quantity: 2,   unit: "slices", isOptional: false },
      { id: "i9", recipeId: "r3", ingredientName: "avocado",   quantity: 1,   unit: "pcs",   isOptional: false },
      { id: "i10", recipeId: "r3", ingredientName: "lemon",    quantity: 1,   unit: "pcs",   isOptional: true  },
    ] as RecipeIngredient[],
  }
];

let nextId = 4;

export const getRecipes = (): Recipe[] => recipes;

export const getRecipeById = (id: string): Recipe | undefined =>
  recipes.find(r => r.id === id);

export const addRecipe = (data: Omit<Recipe, "id">): Recipe => {
  const recipe: Recipe = { ...data, id: `r${nextId++}` };
  recipes.push(recipe);
  return recipe;
};

export const getMissingIngredients = (_recipeId: string, _inventoryNames: string[]): string[] => {
  // Milestone 2: stock matching done client-side; return empty
  return [];
};