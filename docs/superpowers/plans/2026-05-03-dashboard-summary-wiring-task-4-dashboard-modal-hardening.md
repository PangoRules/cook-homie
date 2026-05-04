# Task 4: Harden dashboard expiring modal open path, fix modal prop hydration, remove debug output, and add frontend tests
**Branch:** `task/m2-1-dashboard-modal-hardening`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-dashboard-summary-wiring-design.md`

# Dashboard Modal Hardening Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Prevent dashboard expiring click path from opening an empty editable modal, remove debug UI/logs, and hydrate modal form when `item` prop changes.

**Architecture:** UI shell stays humble: `pages/index.vue` resolves real item details and handles missing lookup with toast. `InventoryItemDetailModal.vue` only mirrors current prop into local form state.

**Tech Stack:** Nuxt 3, Vue 3 composition API, Vitest, Vue Test Utils.

---

## Files

- Modify: `src/CookHomie.Web/pages/index.vue`
- Modify: `src/CookHomie.Web/components/inventory/InventoryItemDetailModal.vue`
- Modify: `src/CookHomie.Web/tests/index.spec.ts`
- Create: `src/CookHomie.Web/tests/InventoryItemDetailModal.spec.ts`
- Modify: `src/CookHomie.Web/tests/helpers/builders.ts` if shared inventory item factory helps tests; keep existing payload factory intact.

## Parallelizable?

Yes. One worker can add `InventoryItemDetailModal` hydration test/fix while another adds dashboard click-path test/fix. Coordinate only on shared builders.

### Task 4.1: Add failing modal hydration test

- [ ] Create `InventoryItemDetailModal.spec.ts`.
- [ ] Mount component with `item: null`, then update prop to real item.
- [ ] Stub shared components from `tests/helpers/stubs.ts` and `Teleport` if needed.
- [ ] Assert input values update to real item values after `await wrapper.setProps({ item })` and `await nextTick()`:
  - name `Milk`
  - category `Dairy`
  - location `Fridge`
  - quantity `1`
  - unit `liter`
- [ ] Run: `npm run test -- InventoryItemDetailModal`
- [ ] Expected: FAIL because watcher observes computed ref wrapper instead of `props.item` value and/or debug `<pre>` renders stale fallback state.

### Task 4.2: Fix modal prop hydration and remove debug output

- [ ] Delete both debug lines from template:

```vue
<pre>{{ localInventoryItem }}</pre>
<pre>{{ item }}</pre>
```

- [ ] Add helper inside `<script setup>`:

```ts
const syncForm = (item: InventoryItem | null) => {
  const source = item ?? defaultItem;
  form.name = source.name;
  form.category = source.category;
  form.location = source.location;
  form.quantity = source.quantity;
  form.unit = source.unit;
  form.isOpened = source.isOpened;
  form.expiresAt = source.expiresAt ?? "";
  form.notes = source.notes ?? "";
};
```

- [ ] Replace current watcher with:

```ts
watch(
  () => props.item,
  (newItem) => syncForm(newItem),
  { immediate: true }
);
```

- [ ] Keep `localInventoryItem = computed(() => props.item ?? defaultItem)` for title fallback only.

### Task 4.3: Verify modal test

- [ ] Run: `npm run test -- InventoryItemDetailModal`
- [ ] Expected: PASS.

### Task 4.4: Add failing dashboard click-path test

- [ ] In `index.spec.ts`, update dashboard fixture objects to use current `DashboardSummary` fields: `expiringItems` and `recipeIdeas`, not `upcomingExpirations`/`recommendedRecipes`.
- [ ] Add test `does not open expiring modal when inventory lookup fails`.
- [ ] Stub `useDashboard` with one expiring row `{ id: "missing-id", name: "Milk", expiresAt: "2026-05-03", location: "Fridge" }`.
- [ ] Stub `useInventory` to return `items: ref([])` and `refresh: vi.fn().mockResolvedValue(undefined)`.
- [ ] Stub `useToast` with `pushError` spy.
- [ ] Mount page with `InventoryItemDetailModal` stub that renders `.detail-modal`.
- [ ] Trigger click on expiring row.
- [ ] Assert `.detail-modal` does not exist and `pushError` called with `"Item details unavailable. Refresh inventory and try again."`.
- [ ] Run: `npm run test -- index`
- [ ] Expected: FAIL because current code sets `showExpiringModal = true` before lookup.

### Task 4.5: Harden `openExpiring` in dashboard page

- [ ] Remove debug console statements from `pages/index.vue`:

```ts
console.log("🚀 index.vue:174 ╎item╎:", item);
console.log("items", items.value);
```

- [ ] Change `openExpiring` behavior:
  - set `selectedExpiringItem` and `expiringItemDetails` to null/reset before lookup
  - call `await refresh()` from `useInventory()`
  - find inventory item by exact ID
  - if missing, keep `showExpiringModal` false and call `useToast().pushError("Item details unavailable. Refresh inventory and try again.")`
  - if found, set `selectedExpiringItem`, `expiringItemDetails`, then `showExpiringModal = true`
- [ ] Final function should be equivalent to:

```ts
const openExpiring = async (item: DashboardExpiringItem) => {
  selectedExpiringItem.value = null;
  expiringItemDetails.value = null;
  showExpiringModal.value = false;

  const { items, refresh: refreshInventory } = useInventory();
  await refreshInventory();
  const details = items.value.find((inventoryItem) => inventoryItem.id === item.id) ?? null;

  if (!details) {
    useToast().pushError("Item details unavailable. Refresh inventory and try again.");
    return;
  }

  selectedExpiringItem.value = item;
  expiringItemDetails.value = details;
  showExpiringModal.value = true;
};
```

### Task 4.6: Add success-path dashboard test

- [ ] Add test `opens expiring modal with real inventory item when lookup succeeds`.
- [ ] Stub inventory items with matching id and real fields.
- [ ] Trigger expiring row click.
- [ ] Assert modal stub exists and receives prop `item.name === "Milk"`, `item.id` equals dashboard row ID.

### Task 4.7: Verify frontend and commit

- [ ] Run: `npm run test -- index InventoryItemDetailModal`
- [ ] Expected: PASS.
- [ ] Run: `npm run validate`
- [ ] Expected: PASS.
- [ ] Commit:

```bash
git add src/CookHomie.Web/pages/index.vue src/CookHomie.Web/components/inventory/InventoryItemDetailModal.vue src/CookHomie.Web/tests/index.spec.ts src/CookHomie.Web/tests/InventoryItemDetailModal.spec.ts src/CookHomie.Web/tests/helpers/builders.ts
git commit -m "fix: guard dashboard expiring modal lookup"
```
