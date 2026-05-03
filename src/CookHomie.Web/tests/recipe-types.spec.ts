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

  it("AddRecipePayload includes ingredients", () => {
    const p: AddRecipePayload = {
      name: "New",
      instructions: "Do",
      prepMinutes: 5,
      cookMinutes: 10,
      tags: [],
      ingredients: [{ ingredientName: "Flour", quantity: 2, unit: "cups", isOptional: false }],
    };
    expect("id" in p).toBe(false);
    expect(p.ingredients).toHaveLength(1);
  });

  it("DashboardSummary has required fields", () => {
    const d: DashboardSummary = {
      expiringCount: 3,
      recipeMatchCount: 2,
      shoppingCount: 4,
      totalItems: 15,
      expiringItems: [
        { id: "inv-1", name: "Milk", expiresAt: "2026-05-03", location: "Fridge" },
      ],
      recipeIdeas: [
        { id: "r1", name: "Classic Pancakes", matchedCount: 2, missingCount: 1 },
      ],
    };
    expect(d.expiringCount).toBe(3);
    expect(d.recipeMatchCount).toBe(2);
    expect(d.shoppingCount).toBe(4);
    expect(d.totalItems).toBe(15);
    expect(d.expiringItems).toHaveLength(1);
    expect(d.expiringItems[0].name).toBe("Milk");
    expect(d.recipeIdeas).toHaveLength(1);
    expect(d.recipeIdeas[0].matchedCount).toBe(2);
    expect(d.recipeIdeas[0].missingCount).toBe(1);
  });
});
