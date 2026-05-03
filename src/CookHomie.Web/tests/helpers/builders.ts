import type { AddInventoryItemPayload, AddRecipeIngredientPayload } from "~/types";

export const makeInventoryItemPayload = (
  overrides: Partial<AddInventoryItemPayload> = {}
): AddInventoryItemPayload => ({
  name: "Milk",
  category: "dairy",
  location: "Fridge",
  quantity: 1,
  unit: "liters",
  isOpened: false,
  ...overrides,
});

export const makeRecipeIngredient = (
  overrides: Partial<AddRecipeIngredientPayload> = {}
): AddRecipeIngredientPayload => ({
  ingredientName: "flour",
  quantity: 200,
  unit: "g",
  isOptional: false,
  ...overrides,
});
