import { describe, expect, it } from "vitest";
import { matchIngredientStock } from "../utils/ingredientStock";

describe("matchIngredientStock", () => {
  it("matches exact trimmed name case-insensitively", () => {
    const inventory = ["Milk", "Eggs", "Butter"];
    expect(matchIngredientStock("milk", inventory)).toBe(true);
    expect(matchIngredientStock("MILK", inventory)).toBe(true);
    expect(matchIngredientStock("  Milk  ", inventory)).toBe(true);
    expect(matchIngredientStock("eggs", inventory)).toBe(true);
  });

  it("returns false for non-matching ingredient", () => {
    const inventory = ["Milk", "Eggs"];
    expect(matchIngredientStock("Flour", inventory)).toBe(false);
  });

  it("handles empty inventory", () => {
    expect(matchIngredientStock("Milk", [])).toBe(false);
  });
});