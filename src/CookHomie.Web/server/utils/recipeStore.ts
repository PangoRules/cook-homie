import type { Recipe } from "~/types";

const recipes: Recipe[] = [
  {
    id: "r1",
    name: "Classic Pancakes",
    instructions: "Mix flour, eggs, milk. Cook on griddle.",
    prepMinutes: 5,
    cookMinutes: 10,
    tags: ["breakfast", "quick"],
    ingredients: []
  },
  {
    id: "r2",
    name: "Tomato Pasta",
    instructions: "Boil pasta. Sauté garlic, add tomatoes, toss.",
    prepMinutes: 5,
    cookMinutes: 15,
    tags: ["pasta", "vegetarian"],
    ingredients: []
  },
  {
    id: "r3",
    name: "Avocado Toast",
    instructions: "Toast bread. Mash avocado with salt and lemon. Spread.",
    prepMinutes: 3,
    cookMinutes: 2,
    tags: ["breakfast", "quick", "vegetarian"],
    ingredients: []
  }
];

let nextId = 4;

export const getRecipes = (): Recipe[] => recipes;

export const getRecipeById = (id: string): Recipe | undefined =>
  recipes.find(r => r.id === id);

export const addRecipe = (data: Omit<Recipe, "id">): Recipe => {
  const recipe: Recipe = { ...data, id: `r${nextId++}`, ingredients: [] };
  recipes.push(recipe);
  return recipe;
};

export const getMissingIngredients = (_recipeId: string, _inventoryNames: string[]): string[] => {
  // Milestone 2: stock matching done client-side; return empty
  return [];
};