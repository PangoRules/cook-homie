# Task 5: Inventory Item Click → View/Edit Modal
**Branch:** `task/task-5-inventory-item-modal`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Create: `src/CookHomie.Web/components/inventory/ItemDetailModal.vue`
- Modify: `src/CookHomie.Web/pages/inventory.vue`
- Modify: `src/CookHomie.Web/pages/index.vue` (expiring items panel)
- Modify: `src/CookHomie.Web/types/index.ts`

## Steps

- [x] **Step 1: Add `EditInventoryItemPayload` type**

In `src/CookHomie.Web/types/index.ts`, add:

```typescript
export type EditInventoryItemPayload = Partial<Omit<InventoryItem, "id">>;
```

- [x] **Step 2: Create `ItemDetailModal.vue`**

- [x] **Step 3: Update `pages/inventory.vue` — add click handler + modal state**

- [x] **Step 4: Update `pages/index.vue` — expiring items click**

- [x] **Step 5: Commit**

```bash
git add src/CookHomie.Web/types/index.ts \
  src/CookHomie.Web/components/inventory/ItemDetailModal.vue \
  src/CookHomie.Web/pages/inventory.vue \
  src/CookHomie.Web/pages/index.vue
git commit -m "feat(web): add InventoryItemDetailModal with inventory and expiring modes"
```
