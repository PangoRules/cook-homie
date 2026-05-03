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

- [ ] **Step 1: Add `EditInventoryItemPayload` type**

In `src/CookHomie.Web/types/index.ts`, add:

```typescript
export type EditInventoryItemPayload = Partial<Omit<InventoryItem, "id">>;
```

- [ ] **Step 2: Create `ItemDetailModal.vue`**

Create `src/CookHomie.Web/components/inventory/ItemDetailModal.vue`. This is a complex component with two modes:

**Props:**
```typescript
interface Props {
  item: InventoryItem;
  mode?: "inventory" | "expiring";
}
```

**`inventory` mode:** view + edit all fields (same fields as `AddItemModal`). Exposes `edited` event with `EditInventoryItemPayload`.

**`expiring` mode:** view + edit + contextual action buttons:
- "Dismiss" — emits `dismiss` (suppress reminder, not a delete)
- "Discard leftover" — emits `discard` with `{ amount: number }` payload
- "Already restocked" — emits `restocked`
- "Add to shopping list" — emits `addToShoppingList`

The component uses `SharedModal` (same as `AddItemModal`). Edit fields mirror `AddItemModal` form. On successful edit, `emit('edited', payload)` and trigger success toast via `useToast`.

```vue
<template>
  <SharedModal ref="modalRef" :title="item.name" max-width="max-w-lg" @close="$emit('close')">
    <form class="flex flex-col gap-4">
      <!-- All AddItemModal fields, pre-populated with item data -->
      <!-- Same layout: name, category+location grid, quantity+unit grid, expiresAt, notes -->
      <SharedFormField label="Name" :error="errors.name" required>
        <input v-model="form.name" class="input" />
      </SharedFormField>
      <!-- ... (same fields as AddItemModal) ... -->
    </form>

    <template #footer>
      <SharedButton variant="secondary" @click="onCancel">Cancel</SharedButton>
      <SharedButton variant="primary" :disabled="saving" @click="handleSave">
        {{ saving ? "Saving…" : "Save Changes" }}
      </SharedButton>
    </template>

    <!-- expiring-mode extra actions below footer slot -->
    <template v-if="mode === 'expiring'" #expiring-actions>
      <div class="flex flex-col gap-2 px-6 pb-4 border-t border-border mt-4">
        <SharedButton variant="secondary" class="w-full" @click="$emit('dismiss')">
          Dismiss
        </SharedButton>
        <SharedButton variant="secondary" class="w-full" @click="showDiscard = true">
          Discard leftover
        </SharedButton>
        <SharedButton variant="secondary" class="w-full" @click="$emit('restocked')">
          Already restocked
        </SharedButton>
        <SharedButton variant="secondary" class="w-full" @click="$emit('addToShoppingList')">
          Add to shopping list
        </SharedButton>
      </div>
    </template>
  </SharedModal>
</template>
```

See the full component implementation in the spec; the pattern mirrors `AddItemModal.vue`.

- [ ] **Step 3: Update `pages/inventory.vue` — add click handler + modal state**

In the `<li>` for each inventory item (line 30–46), add `@click="openItem(item)"` and `cursor-pointer` class. Add modal open state:

```typescript
const selectedItem = ref<InventoryItem | null>(null);
const showDetailModal = ref(false);

const openItem = (item: InventoryItem) => {
  selectedItem.value = item;
  showDetailModal.value = true;
};

const handleEdited = () => {
  showDetailModal.value = false;
  refresh();
};
```

At the bottom of the template, add:

```vue
<InventoryItemDetailModal
  v-if="selectedItem"
  :item="selectedItem"
  mode="inventory"
  @close="showDetailModal = false"
  @edited="handleEdited"
/>
```

- [ ] **Step 4: Update `pages/index.vue` — expiring items click**

In the expiring items loop (lines 55–59), add `@click="openExpiring(item)"` and `cursor-pointer` class. Import the same modal component. Add state for selected expiring item.

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/types/index.ts \
  src/CookHomie.Web/components/inventory/ItemDetailModal.vue \
  src/CookHomie.Web/pages/inventory.vue \
  src/CookHomie.Web/pages/index.vue
git commit -m "feat(web): add InventoryItemDetailModal with inventory and expiring modes"
```
