// src/CookHomie.Web/tests/recipe-types.spec.ts
import { describe, expect, it } from "vitest";
import type { Recipe, AddRecipePayload } from "../types";

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
});
