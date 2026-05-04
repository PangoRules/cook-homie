import { getShoppingItems } from "~/server/utils/shoppingStore";

export default defineEventHandler(() => {
  return getShoppingItems();
});
