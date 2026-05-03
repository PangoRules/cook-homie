import { updateShoppingItem } from "~/server/utils/shoppingStore";

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, "id");
  const body = await readBody(event);

  const updated = updateShoppingItem(id!, body);
  if (!updated) {
    throw createError({ statusCode: 404, statusMessage: "Shopping item not found" });
  }
  return updated;
});