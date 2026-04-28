export interface RecipeIngredient {
  id: string;
  recipeId: string;
  ingredientName: string;
  quantity: number;
  unit: string;
  isOptional: boolean;
}

export interface DashboardSummary {
  expiringCount: number;
  recipeMatchCount: number;
  shoppingCount: number;
}

export type AddRecipePayload = Omit<Recipe, "id" | "ingredients">;

export interface InventoryItem {
  id: string;
  name: string;
  category: string;
  location: string;
  quantity: number;
  unit: string;
  expiresAt?: string;
  isOpened: boolean;
  notes?: string;
}

export interface ShoppingItem {
  id: string;
  name: string;
  quantity?: number;
  unit?: string;
  priority: "low" | "medium" | "high";
  isBought: boolean;
  linkedRecipeId?: string;
}

export interface Recipe {
  id: string;
  name: string;
  instructions: string;
  prepMinutes: number;
  cookMinutes: number;
  tags: string[];
  source?: string;
  ingredients: RecipeIngredient[];
}

export type AddInventoryItemPayload = Omit<InventoryItem, "id">;
export type AddShoppingItemPayload = Omit<ShoppingItem, "id">;

export type AppEnv = "development" | "staging" | "production";
