import type { ShoppingItem } from "~/types";

const store: ShoppingItem[] = [];

export const getShoppingItems = (): ShoppingItem[] => store;

export const addShoppingItem = (item: Omit<ShoppingItem, "id">): ShoppingItem => {
  const newItem: ShoppingItem = {
    ...item,
    id: crypto.randomUUID(),
  };
  store.push(newItem);
  return newItem;
};

export const addShoppingItems = (items: Omit<ShoppingItem, "id">[]): ShoppingItem[] => {
  return items.map(addShoppingItem);
};

export const updateShoppingItem = (
  id: string,
  updates: Partial<ShoppingItem>
): ShoppingItem | null => {
  const idx = store.findIndex((s) => s.id === id);
  if (idx === -1) return null;
  store[idx] = { ...store[idx], ...updates };
  return store[idx];
};

export const deleteShoppingItem = (id: string): boolean => {
  const idx = store.findIndex((s) => s.id === id);
  if (idx === -1) return false;
  store.splice(idx, 1);
  return true;
};

export const getShoppingItemByName = (
  name: string,
  excludeBought = true
): ShoppingItem | undefined =>
  store.find((s) => s.name.toLowerCase() === name.toLowerCase() && (!excludeBought || !s.isBought));
