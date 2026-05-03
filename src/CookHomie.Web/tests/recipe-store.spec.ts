import { describe, expect, it } from "vitest";
import { getRecipes, getRecipeById, addRecipe } from "../server/utils/recipeStore";

describe("recipeStore", () => {
  it("getRecipes returns array", () => {
    const recipes = getRecipes();
    expect(Array.isArray(recipes)).toBe(true);
    expect(recipes.length).toBeGreaterThan(0);
  });

  it("getRecipeById returns recipe or undefined", () => {
    const r = getRecipeById("r1");
    expect(r?.name).toBe("Classic Pancakes");
    expect(getRecipeById("nonexistent")).toBeUndefined();
  });

  it("addRecipe returns recipe with new id", () => {
    const before = getRecipes().length;
    const newR = addRecipe({ name: "Test", instructions: "Test", prepMinutes: 1, cookMinutes: 1, tags: [], ingredients: [] });
    expect(newR.id).toBeTruthy();
    expect(getRecipes().length).toBe(before + 1);
  });

  it("addRecipe persists ingredients when provided", () => {
    const ingredients = [
      { 
        id: "test-ingredient-1", 
        recipeId: "temp", 
        ingredientName: "flour", 
        quantity: 200, 
        unit: "g", 
        isOptional: false 
      }
    ];
    
    const newR = addRecipe({ 
      name: "Test Recipe with Ingredients", 
      instructions: "Test instructions", 
      prepMinutes: 1, 
      cookMinutes: 1, 
      tags: [], 
      ingredients 
    });
    
    expect(newR.ingredients).toEqual(ingredients);
  });
});