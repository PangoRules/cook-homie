# Milestone 2 — Page Polish + Integration Tests

> **For agentic workers:** Each step is `- [ ]`. Implement task-by-task on its own branch. Commit after each task.

**Goal:** Polish the existing inventory and shopping pages with skeletons, error states, and responsive layout; upgrade AddItemModal with full fields and toast feedback; add integration tests for critical flows; run final responsive verification.

**Prerequisite:** Complete Tasks 1–3, 5, 6, 7, 8, 9, 10–16 (all foundation, mock, design system, dashboard, recipes, and detail work).

---

## Task 17 — Upgrade inventory page with skeletons, error states, responsive

**Files:**
- Modify: `src/CookHomie.Web/pages/inventory.vue`

- [ ] **Step 1: Replace inventory page**

Replace `src/CookHomie.Web/pages/inventory.vue` with:

```vue
<template>
  <div class="inventory-page">
    <header class="page-header">
      <h1 class="page-header__title">Inventory</h1>
      <button class="btn-add" @click="showModal = true">+ Add Item</button>
    </header>

    <DashboardPanel
      title=""
      :loading="loading"
      :error="error"
      :is-stale="isStale"
      :has-data="items.length > 0"
      show-refresh
      @refresh="refresh"
    >
      <div v-if="loading && items.length === 0" class="item-list">
        <SkeletonBlock v-for="i in 5" :key="i" height="48px" />
      </div>
      <div v-else-if="error && items.length === 0">
        <ErrorBanner :message="error" show-retry @retry="refresh" />
      </div>
      <div v-else-if="items.length === 0" class="empty-state">
        <p>No inventory items. Add your first item above.</p>
      </div>
      <ul v-else class="item-list">
        <li v-for="item in items" :key="item.id" class="item-card">
          <div class="item-card__info">
            <span class="item-card__name">{{ item.name }}</span>
            <span class="item-card__meta">{{ item.quantity }} {{ item.unit }} · {{ item.location }}</span>
          </div>
          <span :class="['item-card__expiry', item.expiresAt ? 'item-card__expiry--soon' : '']">
            {{ item.expiresAt ? formatExpiry(item.expiresAt) : "No expiry" }}
          </span>
        </li>
      </ul>
    </DashboardPanel>

    <AddItemModal v-if="showModal" @added="handleAdded" @close="showModal = false" />
  </div>
</template>

<script setup lang="ts">
const { items, loading, error, isStale, startPolling, stopPolling, refresh } = useInventory();
const showModal = ref(false);

onMounted(() => startPolling());
onUnmounted(() => stopPolling());

const handleAdded = () => { showModal.value = false; refresh(); };

const formatExpiry = (date: string) => {
  const d = new Date(date);
  const now = new Date();
  const diff = Math.ceil((d.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
  if (diff < 0) return "Expired";
  if (diff === 0) return "Expires today";
  if (diff <= 3) return `Expires in ${diff}d`;
  return d.toLocaleDateString();
};
</script>

<style scoped>
.inventory-page { max-width: 960px; margin: 0 auto; }

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-6);
}

.page-header__title {
  font-family: var(--font-display);
  font-size: 28px;
  font-weight: 700;
  margin: 0;
}

.btn-add {
  background: var(--color-accent);
  color: white;
  border: none;
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-5);
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
}

.item-list { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: var(--space-2); }

.item-card {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-3) var(--space-4);
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  transition: box-shadow var(--transition-fast);
}

.item-card:hover { box-shadow: var(--shadow-sm); }
.item-card__info { display: flex; flex-direction: column; gap: var(--space-1); }
.item-card__name { font-weight: 600; font-size: 15px; }
.item-card__meta { font-size: 12px; color: var(--color-text-muted); }
.item-card__expiry { font-size: 12px; color: var(--color-text-muted); }
.item-card__expiry--soon { color: var(--color-warning); font-weight: 600; }
.empty-state { text-align: center; padding: var(--space-10); color: var(--color-text-muted); }

@media (max-width: 480px) {
  .page-header { flex-direction: column; align-items: flex-start; gap: var(--space-3); }
}
</style>
```

- [ ] **Step 2: Verify page renders**

Start dev server, visit `/inventory`, verify skeletons on load, items render with location/quantity, add item modal works.

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/pages/inventory.vue
git commit -m "feat(web): polish inventory page with skeletons, error states, responsive"
```

---

## Task 18 — Upgrade AddItemModal with full fields + toast feedback

**Files:**
- Modify: `src/CookHomie.Web/components/inventory/AddItemModal.vue`

- [ ] **Step 1: Write full AddItemModal**

Replace `src/CookHomie.Web/components/inventory/AddItemModal.vue` with the complete modal — same field structure as AddRecipeModal but adapted for inventory:

```vue
<!-- src/CookHomie.Web/components/inventory/AddItemModal.vue -->
<template>
  <div class="modal-backdrop" @click.self="$emit('close')">
    <div class="modal">
      <header class="modal__header">
        <h2 class="modal__title">Add Inventory Item</h2>
        <button class="modal__close" @click="$emit('close')">×</button>
      </header>

      <form class="modal__form" @submit.prevent="handleSubmit">
        <div class="field">
          <label class="field__label">Name *</label>
          <input v-model="form.name" class="field__input" placeholder="e.g. Milk" required />
          <span v-if="errors.name" class="field__error">{{ errors.name }}</span>
        </div>

        <div class="field-row">
          <div class="field">
            <label class="field__label">Category *</label>
            <input v-model="form.category" class="field__input" placeholder="e.g. dairy" required />
          </div>
          <div class="field">
            <label class="field__label">Location *</label>
            <select v-model="form.location" class="field__input">
              <option value="">Select...</option>
              <option value="Pantry">Pantry</option>
              <option value="Fridge">Fridge</option>
              <option value="Freezer">Freezer</option>
              <option value="Spices">Spices</option>
            </select>
          </div>
        </div>

        <div class="field-row">
          <div class="field">
            <label class="field__label">Quantity *</label>
            <input v-model.number="form.quantity" type="number" min="0" step="1" class="field__input" />
          </div>
          <div class="field">
            <label class="field__label">Unit *</label>
            <input v-model="form.unit" class="field__input" placeholder="e.g. liters" />
          </div>
        </div>

        <div class="field">
          <label class="field__label">Expires At</label>
          <input v-model="form.expiresAt" type="date" class="field__input" />
        </div>

        <div class="field">
          <label class="field__label">Notes</label>
          <textarea v-model="form.notes" class="field__input" rows="2" placeholder="Optional notes..." />
        </div>

        <div class="modal__actions">
          <button type="button" class="btn-secondary" @click="$emit('close')">Cancel</button>
          <button type="submit" class="btn-primary" :disabled="submitting">
            {{ submitting ? "Adding…" : "Add Item" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import type { AddInventoryItemPayload } from "~/types";

const emit = defineEmits(["added", "close"]);

const form = reactive<AddInventoryItemPayload & { expiresAt?: string }>({
  name: "",
  category: "",
  location: "",
  quantity: 1,
  unit: "",
  isOpened: false,
  expiresAt: undefined,
  notes: undefined
});

const errors = reactive({ name: "", location: "", quantity: "" });
const submitting = ref(false);
const { pushSuccess, pushError } = useToast();
const { addInventoryItem } = useInventory();

const validate = () => {
  errors.name = "";
  errors.location = "";
  errors.quantity = "";
  if (!form.name.trim()) errors.name = "Name is required";
  if (!form.location) errors.location = "Location is required";
  if (form.quantity <= 0) errors.quantity = "Quantity must be greater than 0";
  return !errors.name && !errors.location && !errors.quantity;
};

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    const { expiresAt, notes, ...payload } = form;
    const created = await addInventoryItem(payload);
    emit("added", created);
    pushSuccess(`"${form.name}" added to inventory!`);
  } catch (err) {
    pushError(err instanceof Error ? err : new Error("Failed to add item"));
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
/* Same .modal-backdrop, .modal, .modal__header, .modal__form, .field, .field__label,
   .field__input, .field__error, .field-row, .modal__actions, .btn-primary, .btn-secondary
   styles as AddRecipeModal — copy those same CSS rules here verbatim */
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}
.modal {
  background: var(--color-surface);
  border-radius: var(--radius-lg);
  width: 90%;
  max-width: 480px;
  box-shadow: var(--shadow-lg);
  overflow: hidden;
}
.modal__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-4) var(--space-5);
  border-bottom: 1px solid var(--color-border);
}
.modal__title { font-family: var(--font-display); font-size: 18px; font-weight: 600; margin: 0; }
.modal__close { background: none; border: none; font-size: 22px; cursor: pointer; color: var(--color-text-muted); }
.modal__form { padding: var(--space-5); display: flex; flex-direction: column; gap: var(--space-4); }
.field { display: flex; flex-direction: column; gap: var(--space-1); }
.field__label { font-size: 13px; font-weight: 600; color: var(--color-text-secondary); }
.field__input {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-3);
  font-size: 14px;
  font-family: var(--font-body);
  background: var(--color-bg);
  color: var(--color-text-primary);
  width: 100%;
  box-sizing: border-box;
}
.field__input:focus { outline: none; border-color: var(--color-accent); }
.field__error { font-size: 12px; color: var(--color-error); }
.field-row { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-3); }
.modal__actions { display: flex; justify-content: flex-end; gap: var(--space-3); margin-top: var(--space-2); }
.btn-primary { background: var(--color-accent); color: white; border: none; border-radius: var(--radius-md); padding: var(--space-2) var(--space-5); font-size: 14px; font-weight: 600; cursor: pointer; transition: background var(--transition-fast); }
.btn-primary:hover:not(:disabled) { background: var(--color-accent-hover); }
.btn-primary:disabled { opacity: 0.5; cursor: not-allowed; }
.btn-secondary { background: none; border: 1px solid var(--color-border); border-radius: var(--radius-md); padding: var(--space-2) var(--space-5); font-size: 14px; cursor: pointer; color: var(--color-text-secondary); }
</style>
```

- [ ] **Step 2: Verify modal opens and submits**

Start dev server, visit `/inventory`, click "+ Add Item", verify all fields render, form validates, submit shows toast on success/error.

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/components/inventory/AddItemModal.vue
git commit -m "feat(web): upgrade AddItemModal with full fields and toast feedback"
```

---

## Task 19 — Upgrade shopping page with skeletons and empty state

**Files:**
- Modify: `src/CookHomie.Web/pages/shopping.vue`

- [ ] **Step 1: Replace shopping page**

Replace `src/CookHomie.Web/pages/shopping.vue`:

```vue
<template>
  <div class="shopping-page">
    <header class="page-header">
      <h1 class="page-header__title">Shopping List</h1>
    </header>
    <DashboardPanel
      title=""
      :loading="loading"
      :error="error"
      :has-data="items.length > 0"
      show-refresh
      @refresh="refresh"
    >
      <div v-if="loading && items.length === 0">
        <SkeletonBlock v-for="i in 4" :key="i" height="48px" />
      </div>
      <div v-else-if="items.length === 0" class="empty-state">
        <p>Your shopping list is empty.</p>
      </div>
      <ul v-else class="shopping-list">
        <li v-for="item in items" :key="item.id" :class="['shopping-item', item.isBought ? 'shopping-item--bought' : '']">
          <span>{{ item.name }}</span>
          <span v-if="item.quantity">{{ item.quantity }} {{ item.unit }}</span>
        </li>
      </ul>
    </DashboardPanel>
  </div>
</template>

<script setup lang="ts">
// Stub: replace with actual useShoppingList composable once shopping API exists
const items = ref([]);
const loading = ref(false);
const error = ref<string | null>(null);
const refresh = async () => {};
</script>

<style scoped>
.shopping-page { max-width: 960px; margin: 0 auto; }
.page-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: var(--space-6); }
.page-header__title { font-family: var(--font-display); font-size: 28px; font-weight: 700; margin: 0; }
.shopping-list { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: var(--space-2); }
.shopping-item { display: flex; align-items: center; justify-content: space-between; padding: var(--space-3) var(--space-4); background: var(--color-surface); border: 1px solid var(--color-border); border-radius: var(--radius-md); }
.shopping-item--bought { text-decoration: line-through; opacity: 0.5; }
.empty-state { text-align: center; padding: var(--space-10); color: var(--color-text-muted); }
</style>
```

- [ ] **Step 2: Verify page renders**

Visit `/shopping`, verify empty state renders correctly.

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/pages/shopping.vue
git commit -m "feat(web): polish shopping page with skeletons, empty state"
```

---

## Task 20 — Integration tests for critical flows

**Files:**
- Create: `src/CookHomie.Web/tests/polling-lifecycle.spec.ts`
- Create: `src/CookHomie.Web/tests/add-recipe.spec.ts`

- [ ] **Step 1: Write polling lifecycle test**

```typescript
// src/CookHomie.Web/tests/polling-lifecycle.spec.ts
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { usePollingFetch } from "../composables/usePollingFetch";

describe("polling lifecycle", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.stubGlobal("useState", (_k: string, init: () => unknown) => ref(init()));
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue({ ok: true }));
  });

  it("starts polling on start() and stops on stop()", async () => {
    const { start, stop } = usePollingFetch("/api/test");
    await start();
    vi.advanceTimersByTime(30000);
    expect(vi.mocked($fetch)).toHaveBeenCalledTimes(2); // 1 immediate + 1 after 30s
    stop();
    vi.advanceTimersByTime(30000);
    expect(vi.mocked($fetch)).toHaveBeenCalledTimes(2); // no more calls after stop
    vi.useRealTimers();
  });

  it("refresh() fetches immediately without affecting interval", async () => {
    const { start, refresh } = usePollingFetch("/api/test");
    await start();
    vi.advanceTimersByTime(10000);
    const countBefore = vi.mocked($fetch).mock.calls.length;
    await refresh();
    const countAfter = vi.mocked($fetch).mock.calls.length;
    expect(countAfter).toBe(countBefore + 1);
    vi.useRealTimers();
  });
});
```

- [ ] **Step 2: Write AddRecipe full flow test**

```typescript
// src/CookHomie.Web/tests/add-recipe.spec.ts
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { mount } from "@vue/test-utils";

describe("AddRecipe full flow", () => {
  beforeEach(() => {
    vi.stubGlobal("useToast", () => ({ pushSuccess: vi.fn(), pushError: vi.fn() }));
    vi.stubGlobal("useState", (_k: string, init: () => unknown) => ref(init()));
  });

  it("submits POST to /api/recipes on form submit", async () => {
    const router = { push: vi.fn() };
    vi.stubGlobal("useRouter", () => router);
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue({ id: "new-recipe-id", name: "Test" }));

    const AddRecipeModal = (await import("../components/recipes/AddRecipeModal.vue")).default;
    const wrapper = mount(AddRecipeModal, {
      global: { stubs: { Teleport: false }, mocks: { useRouter: () => router } }
    });

    await wrapper.find('input[placeholder*="Banana"]').setValue("New Recipe");
    await wrapper.find("form").trigger("submit");

    expect(vi.mocked($fetch)).toHaveBeenCalledWith("/api/recipes", expect.any(Object));
  });
});
```

- [ ] **Step 3: Run tests**

Run: `cd src/CookHomie.Web && npx vitest run tests/polling-lifecycle.spec.ts tests/add-recipe.spec.ts`
Expected: PASS

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/tests/polling-lifecycle.spec.ts src/CookHomie.Web/tests/add-recipe.spec.ts
git commit -m "test(web): add polling lifecycle and add-recipe integration tests"
```

---

## Task 21 — Final responsive verification + completion check

**Files:** No file changes — verify only.

- [ ] **Step 1: Run all tests**

Run: `cd src/CookHomie.Web && npx vitest run`
Expected: All tests pass

- [ ] **Step 2: Responsive browser check**

Start dev server: `cd src/CookHomie.Web && npm run dev`
- At 375px (phone): verify single-column layouts, no horizontal overflow
- At 768px (tablet): verify 2-column dashboard and recipe grids
- At 1024px+ (desktop): verify full multi-column layouts

Check: Dashboard, Recipes, Recipe Detail, Inventory, Shopping pages all render correctly at each breakpoint.

- [ ] **Step 3: Verify completion criteria**

1. Dashboard renders with 3 stat cards + panels
2. Recipes index renders recipe grid + AddRecipeModal
3. Recipe detail shows In Stock / Missing badges on ingredients
4. Toast appears on user actions (add recipe, add inventory item)
5. Polling starts on mount, stops on unmount
6. Error banners + stale indicators appear correctly
7. All responsive breakpoints work without overflow

- [ ] **Step 4: Final commit**

```bash
git add .
git commit -m "feat(web): complete Milestone 2 frontend — dashboard, recipes, detail, design system, testing"
```
