import { describe, it, beforeEach, afterEach } from 'vitest';
import { RecipeStore } from './recipeStore';
import { ApiClient } from '../api_client';
import { mock, mockDeep } from 'vitest-mock-extended';

// Mock the ApiClient
const mockApiClient = mockDeep<ApiClient>();

vi.mock('../api_client', () => ({
  ApiClient: vi.fn().mockImplementation(() => mockApiClient)
}));

describe('RecipeStore', () => {
  beforeEach(() => {
    // Clear all mocks before each test
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('addRecipe', () => {
    it('should add a recipe with ingredients and return the recipe with assigned IDs', async () => {
      const recipeInput = {
        name: 'Test Recipe',
        ingredients: [
          { name: 'Tomato', quantity: 2, unit: 'pieces' },
          { name: 'Basil', quantity: 10, unit: 'leaves' }
        ],
        instructions: 'Mix ingredients together'
      };

      const recipeOutput = {
        id: '123e4567-e89b-12d3-a456-426614174000',
        name: 'Test Recipe',
        ingredients: [
          { id: 'ingredient1-id', name: 'Tomato', quantity: 2, unit: 'pieces' },
          { id: 'ingredient2-id', name: 'Basil', quantity: 10, unit: 'leaves' }
        ],
        instructions: 'Mix ingredients together'
      };

      mockApiClient.addRecipe.mockResolvedValue(recipeOutput);

      const result = await RecipeStore.addRecipe(recipeInput);

      expect(result).toEqual(recipeOutput);
      expect(mockApiClient.addRecipe).toHaveBeenCalledWith(recipeInput);
    });

    it('should handle recipe with ingredients that already have IDs', async () => {
      const recipeInput = {
        name: 'Test Recipe with existing IDs',
        ingredients: [
          { id: 'existing-id-1', name: 'Tomato', quantity: 2, unit: 'pieces' },
          { id: 'existing-id-2', name: 'Basil', quantity: 10, unit: 'leaves' }
        ],
        instructions: 'Mix ingredients together'
      };

      const recipeOutput = {
        id: '123e4567-e89b-12d3-a456-426614174001',
        name: 'Test Recipe with existing IDs',
        ingredients: recipeInput.ingredients,
        instructions: 'Mix ingredients together'
      };

      mockApiClient.addRecipe.mockResolvedValue(recipeOutput);

      const result = await RecipeStore.addRecipe(recipeInput);

      expect(result).toEqual(recipeOutput);
      expect(mockApiClient.addRecipe).toHaveBeenCalledWith(recipeInput);
    });

    it('should handle recipe with no ingredients', async () => {
      const recipeInput = {
        name: 'Test Recipe No Ingredients',
        ingredients: [],
        instructions: 'No ingredients needed'
      };

      const recipeOutput = {
        id: '123e4567-e89b-12d3-a456-426614174002',
        name: 'Test Recipe No Ingredients',
        ingredients: [],
        instructions: 'No ingredients needed'
      };

      mockApiClient.addRecipe.mockResolvedValue(recipeOutput);

      const result = await RecipeStore.addRecipe(recipeInput);

      expect(result).toEqual(recipeOutput);
      expect(mockApiClient.addRecipe).toHaveBeenCalledWith(recipeInput);
    });
  });
});