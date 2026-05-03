import { addShoppingItems, getShoppingItemByName } from "~/server/utils/shoppingStore";

export default defineEventHandler(async (event) => {
  const body = await readBody<{ ingredientNames: string[] }>(event);
  const { ingredientNames } = body;

  const toAdd = ingredientNames.filter((name) => !getShoppingItemByName(name));
  if (toAdd.length === 0) {
    throw createError({ statusCode: 409, statusMessage: "All items already on list" });
  }

  return addShoppingItems(toAdd.map((name) => ({
    name,
    priority: "medium",
    isBought: false,
  })));
});