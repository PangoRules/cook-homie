export interface InventoryItem {
  id: string;
  name: string;
  quantity: number;
  unit: string;
  expiryDate?: Date;
  tags: string[];
  createdAt: Date;
  updatedAt: Date;
}

export interface ShoppingItem {
  id: string;
  name: string;
  quantity: number;
  unit: string;
  isBought: boolean;
  tags: string[];
  createdAt: Date;
  updatedAt: Date;
}

export interface RecipeIngredient {
  id: string;
  name: string;
  quantity: number;
  unit: string;
  tags: string[];
}

export interface Recipe {
  id: string;
  name: string;
  description: string;
  ingredients: RecipeIngredient[];
  instructions: string;
  prepTime: number;
  cookTime: number;
  tags: string[];
  createdAt: Date;
  updatedAt: Date;
}

export interface InventoryItemInput {
  name: string;
  quantity: number;
  unit: string;
  expiryDate?: Date;
  tags: string[];
}

export interface ShoppingItemInput {
  name: string;
  quantity: number;
  unit: string;
  tags: string[];
}

export interface RecipeInput {
  name: string;
  description: string;
  ingredients: RecipeIngredient[];
  instructions: string;
  prepTime: number;
  cookTime: number;
  tags: string[];
}