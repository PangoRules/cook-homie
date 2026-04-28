export const matchIngredientStock = (
  ingredientName: string,
  inventoryNames: string[]
): boolean => {
  const normalized = ingredientName.trim().toLowerCase();
  return inventoryNames.some(n => n.trim().toLowerCase() === normalized);
};