// src/CookHomie.Web/tests/recipe-types.spec.ts
import { describe, expect, it } from "vitest";
import type { Recipe, AddRecipePayload, DashboardSummary } from "../types";

describe("Recipe types", () => {
  it("Recipe has required fields", () => {
    const r: Recipe = {
      id: "1",
      name: "Test",
      instructions: "Step 1",
      prepMinutes: 10,
      cookMinutes: 20,
      tags: ["quick"],
      ingredients: [],
    };
    expect(r.name).toBe("Test");
    expect(r.prepMinutes).toBe(10);
  });

  it("AddRecipePayload omits id", () => {
    const p: AddRecipePayload = {
      name: "New",
      instructions: "Do",
      prepMinutes: 5,
      cookMinutes: 10,
      tags: [],
    };
    expect("id" in p).toBe(false);
  });

  it("DashboardSummary has required fields", () => {
    const d: DashboardSummary = {
      expiringCount: 3,
      recipeMatchCount: 7,
      shoppingCount: 4,
      totalItems: 15,
      upcomingExpirations: ["2024-05-15", "2024-05-20", "2024-05-25"],
      recommendedRecipes: ["Spaghetti", "Chicken Salad", "Vegetable Stir Fry"]
    };
    expect(d.expiringCount).toBe(3);
    expect(d.recipeMatchCount).toBe(7);
    expect(d.shoppingCount).toBe(4);
    expect(d.totalItems).toBe(15);
    expect(d.upcomingExpirations).toHaveLength(3);
    expect(d.recommendedRecipes).toHaveLength(3);
  });
});
