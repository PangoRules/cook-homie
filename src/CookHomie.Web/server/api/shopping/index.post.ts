import { addShoppingItems } from "~/server/utils/shoppingStore";
import type { AddShoppingItemPayload } from "~/types";

export default defineEventHandler(async (event) => {
  const body = await readBody<AddShoppingItemPayload[]>(event);
  // Supports single item or array
  const items = Array.isArray(body) ? body : [body];
  return addShoppingItems(items);
});