export interface RecipeIngredient {
  id: string;
  recipeId: string;
  ingredientName: string;
  quantity: number;
  unit: string;
  isOptional: boolean;
  isInStock?: boolean;
}

export interface DashboardExpiringItem {
  id: string;
  name: string;
  expiresAt: string;
  location: string;
}

export interface DashboardRecipeIdea {
  id: string;
  name: string;
  matchedCount: number;
  missingCount: number;
}

export interface DashboardSummary {
  expiringCount: number;
  recipeMatchCount: number;
  shoppingCount: number;
  totalItems: number;
  expiringItems: DashboardExpiringItem[];
  recipeIdeas: DashboardRecipeIdea[];
}

export type AddRecipeIngredientPayload = Omit<RecipeIngredient, "id" | "recipeId" | "isInStock">;

export type AddRecipePayload = Omit<Recipe, "id"> & {
  ingredients: AddRecipeIngredientPayload[];
};

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
