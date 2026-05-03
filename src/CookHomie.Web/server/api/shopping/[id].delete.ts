import { deleteShoppingItem } from "~/server/utils/shoppingStore";

export default defineEventHandler((event) => {
  const id = getRouterParam(event, "id");
  const deleted = deleteShoppingItem(id!);
  if (!deleted) {
    throw createError({ statusCode: 404, statusMessage: "Shopping item not found" });
  }
  return { success: true };
});