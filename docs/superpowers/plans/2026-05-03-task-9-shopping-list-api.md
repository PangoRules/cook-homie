# Task 9: Shopping List API — Full CRUD + Bulk + Composable
**Branch:** `task/task-9-shopping-list-api`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Create: `src/CookHomie.Web/server/api/shopping/index.get.ts`
- Create: `src/CookHomie.Web/server/api/shopping/index.post.ts`
- Create: `src/CookHomie.Web/server/api/shopping/[id].patch.ts`
- Create: `src/CookHomie.Web/server/api/shopping/[id].delete.ts`
- Create: `src/CookHomie.Web/server/api/shopping/bulk.post.ts`
- Create: `src/CookHomie.Web/server/utils/shoppingStore.ts`
- Modify: `src/CookHomie.Web/composables/useShoppingList.ts`

## Dependencies
- This task is foundational — it unblocks Tasks 6 and 8. Complete this before Tasks 6 and 8.

## Steps

- [ ] **Step 1: Create `server/utils/shoppingStore.ts`**

In-memory store (MVP). Follow the same pattern as `recipeStore.ts` and `inventoryStore.ts` if they exist. Basic CRUD on `ShoppingItem[]` array.

```typescript
import type { ShoppingItem } from "~/types";

let store: ShoppingItem[] = [];

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

export const updateShoppingItem = (id: string, updates: Partial<ShoppingItem>): ShoppingItem | null => {
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

export const getShoppingItemByName = (name: string, excludeBought = true): ShoppingItem | undefined =>
  store.find((s) => s.name.toLowerCase() === name.toLowerCase() && (!excludeBought || !s.isBought));
```

- [ ] **Step 2: Create `server/api/shopping/index.get.ts`**

```typescript
import { getShoppingItems } from "~/server/utils/shoppingStore";

export default defineEventHandler(() => {
  return getShoppingItems();
});
```

- [ ] **Step 3: Create `server/api/shopping/index.post.ts`**

```typescript
import { addShoppingItems } from "~/server/utils/shoppingStore";
import type { AddShoppingItemPayload } from "~/types";

export default defineEventHandler(async (event) => {
  const body = await readBody<AddShoppingItemPayload[]>(event);
  // Supports single item or array
  const items = Array.isArray(body) ? body : [body];
  return addShoppingItems(items);
});
```

- [ ] **Step 4: Create `server/api/shopping/bulk.post.ts`**

For bulk add of missing ingredients. Deduplication at server level:

```typescript
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
```

- [ ] **Step 5: Create `server/api/shopping/[id].patch.ts`**

```typescript
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
```

- [ ] **Step 6: Create `server/api/shopping/[id].delete.ts`**

```typescript
import { deleteShoppingItem } from "~/server/utils/shoppingStore";

export default defineEventHandler((event) => {
  const id = getRouterParam(event, "id");
  const deleted = deleteShoppingItem(id!);
  if (!deleted) {
    throw createError({ statusCode: 404, statusMessage: "Shopping item not found" });
  }
  return { success: true };
});
```

- [ ] **Step 7: Enhance `useShoppingList.ts` composable**

Add full CRUD API:

```typescript
export const useShoppingList = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<ShoppingItem[]>(
    API_ROUTES.SHOPPING.LIST,
    { pollIntervalMs: 30000 }
  );

  const items = computed<ShoppingItem[]>(() => data.value ?? []);

  // POST single item
  const addItem = async (payload: Omit<ShoppingItem, "id">): Promise<ShoppingItem> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<ShoppingItem>(API_ROUTES.SHOPPING.LIST, {
        method: "POST",
        body: payload,
      });
      data.value = [...(data.value ?? []), created];
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // POST bulk
  const addBulk = async (ingredientNames: string[]): Promise<ShoppingItem[]> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<ShoppingItem[]>(API_ROUTES.SHOPPING.BULK, {
        method: "POST",
        body: { ingredientNames },
      });
      data.value = [...(data.value ?? []), ...created];
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add items";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // PATCH (toggle bought, update qty/priority)
  const updateItem = async (id: string, updates: Partial<ShoppingItem>): Promise<ShoppingItem> => {
    loading.value = true;
    error.value = null;
    try {
      const updated = await $fetch<ShoppingItem>(API_ROUTES.SHOPPING.DETAIL(id), {
        method: "PATCH",
        body: updates,
      });
      data.value = (data.value ?? []).map((s) => s.id === id ? updated : s);
      return updated;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to update item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // DELETE
  const removeItem = async (id: string): Promise<void> => {
    loading.value = true;
    error.value = null;
    try {
      await $fetch(API_ROUTES.SHOPPING.DETAIL(id), { method: "DELETE" });
      data.value = (data.value ?? []).filter((s) => s.id !== id);
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to remove item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // Toggle bought
  const toggleBought = async (id: string) => {
    const item = items.value.find((s) => s.id === id);
    if (!item) return;
    await updateItem(id, { isBought: !item.isBought });
  };

  return {
    items,
    loading,
    error,
    isStale,
    loadList: refresh,
    startPolling: start,
    stopPolling: stop,
    refresh,
    addItem,
    addBulk,
    updateItem,
    removeItem,
    toggleBought,
  };
};
```

- [ ] **Step 8: Commit**

```bash
git add src/CookHomie.Web/server/api/shopping/index.get.ts \
  src/CookHomie.Web/server/api/shopping/index.post.ts \
  src/CookHomie.Web/server/api/shopping/[id].patch.ts \
  src/CookHomie.Web/server/api/shopping/[id].delete.ts \
  src/CookHomie.Web/server/api/shopping/bulk.post.ts \
  src/CookHomie.Web/server/utils/shoppingStore.ts \
  src/CookHomie.Web/composables/useShoppingList.ts
git commit -m "feat(web): shopping list full CRUD + bulk API and composable"
```
