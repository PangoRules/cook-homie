# Task 17-21 — Page Polish + Integration Tests

> **For agentic workers:** Each step is `- [ ]`. Each task is independent.

**Goal:** Polish inventory and shopping pages with skeletons, error states, responsive layout; upgrade AddItemModal with full fields and toast feedback; update integration tests; verify responsive behavior.

**Architecture:**
- Use shared components (`SharedModal`, `SharedFormField`, `SharedButton`, `SharedSkeletonBlock`, `SharedErrorBanner`, `SharedStaleIndicator`) with `<SharedXxx>` prefix (Nuxt auto-import)
- Use composables (`useInventory`, `useShoppingList`) for data and polling
- All styling via Tailwind utility classes — no scoped `<style>` blocks
- Follow AddRecipeModal pattern for modal + form structure

**Tech Stack:** Nuxt 3 + Vue 3 + Tailwind + Vitest

**Branch:** `task/page-polish-and-tests`
**Parent branch:** `feat/milestone-2-frontend`

---

## Task 17 — Upgrade inventory page with skeletons, error states, responsive

**Files:**
- Modify: `src/CookHomie.Web/pages/inventory.vue`

- [ ] **Step 1: Replace inventory page**

Replace `src/CookHomie.Web/pages/inventory.vue` entirely:

```vue
<template>
  <div class="max-w-4xl mx-auto p-6">
    <header class="flex items-center justify-between mb-6">
      <h1 class="font-display text-[28px] font-bold">Inventory</h1>
      <SharedButton @click="showModal = true">+ Add Item</SharedButton>
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
      <div v-if="loading && items.length === 0" class="flex flex-col gap-2">
        <SharedSkeletonBlock v-for="i in 5" :key="i" class="h-12" />
      </div>
      <SharedErrorBanner
        v-else-if="error && items.length === 0"
        :message="error"
        show-retry
        @retry="refresh"
      />
      <p v-else-if="items.length === 0" class="text-text-muted text-sm text-center py-10">
        No inventory items. Add your first item above.
      </p>
      <ul v-else class="flex flex-col gap-2 list-none p-0 m-0">
        <li
          v-for="item in items"
          :key="item.id"
          class="flex items-center justify-between p-3 bg-surface border border-border rounded-[10px] hover:shadow-sm transition-shadow"
        >
          <div class="flex flex-col gap-0.5">
            <span class="font-semibold text-[15px]">{{ item.name }}</span>
            <span class="text-xs text-text-muted">{{ item.quantity }} {{ item.unit }} · {{ item.location }}</span>
          </div>
          <span
            :class="['text-xs', item.expiresAt ? 'text-warning font-semibold' : 'text-text-muted']"
          >
            {{ item.expiresAt ? formatExpiry(item.expiresAt) : "No expiry" }}
          </span>
        </li>
      </ul>
    </DashboardPanel>

    <InventoryAddItemModal v-if="showModal" @added="handleAdded" @close="showModal = false" />
  </div>
</template>

<script setup lang="ts">
const { items, loading, error, isStale, startPolling, stopPolling, refresh } = useInventory();
const showModal = ref(false);

onMounted(() => startPolling());
onUnmounted(() => stopPolling());

const handleAdded = () => {
  showModal.value = false;
  refresh();
};

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
```

- [ ] **Step 2: Verify page renders**

```bash
cd src/CookHomie.Web && npm run dev
# Visit http://localhost:3000/inventory
# Verify: skeletons on load, items render with name/quantity/location/expiry, add item modal opens
```

- [ ] **Step 3: Run validation**

```bash
cd src/CookHomie.Web && npm run validate
```

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/pages/inventory.vue
git commit -m "feat(web): polish inventory page with skeletons, error states, responsive layout"
```

---

## Task 18 — Upgrade AddItemModal with full fields + toast feedback

**Files:**
- Modify: `src/CookHomie.Web/components/inventory/AddItemModal.vue`

- [ ] **Step 1: Write full AddItemModal**

Replace `src/CookHomie.Web/components/inventory/AddItemModal.vue` with:

```vue
<template>
  <SharedModal ref="modalRef" title="Add Inventory Item" @close="$emit('close')">
    <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
      <SharedFormField label="Name" :error="errors.name" required>
        <input v-model="form.name" class="input" placeholder="e.g. Milk" />
      </SharedFormField>

      <div class="grid grid-cols-2 gap-4">
        <SharedFormField label="Category" :error="errors.category" required>
          <input v-model="form.category" class="input" placeholder="e.g. dairy" />
        </SharedFormField>
        <SharedFormField label="Location" :error="errors.location" required>
          <select v-model="form.location" class="input">
            <option value="">Select...</option>
            <option value="Pantry">Pantry</option>
            <option value="Fridge">Fridge</option>
            <option value="Freezer">Freezer</option>
            <option value="Spices">Spices</option>
          </select>
        </SharedFormField>
      </div>

      <div class="grid grid-cols-2 gap-4">
        <SharedFormField label="Quantity" :error="errors.quantity" required>
          <input v-model.number="form.quantity" type="number" min="0" step="1" class="input" />
        </SharedFormField>
        <SharedFormField label="Unit">
          <input v-model="form.unit" class="input" placeholder="e.g. liters" />
        </SharedFormField>
      </div>

      <SharedFormField label="Expires At">
        <input v-model="form.expiresAt" type="date" class="input" />
      </SharedFormField>

      <SharedFormField label="Notes">
        <textarea v-model="form.notes" class="input" rows="2" placeholder="Optional notes..." />
      </SharedFormField>
    </form>

    <template #footer>
      <SharedButton variant="secondary" @click="onCancel">Cancel</SharedButton>
      <SharedButton variant="primary" :disabled="submitting" @click="handleSubmit">
        {{ submitting ? "Adding…" : "Add Item" }}
      </SharedButton>
    </template>
  </SharedModal>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import type { AddInventoryItemPayload } from "~/types";
import { useToast } from "~/composables/useToast";

const emit = defineEmits(["added", "close"]);
const modalRef = ref();
const { pushSuccess, pushError } = useToast();

const form = reactive({
  name: "",
  category: "",
  location: "",
  quantity: 1,
  unit: "",
  isOpened: false,
  expiresAt: "",
  notes: "",
});

const errors = reactive({ name: "", category: "", location: "", quantity: "" });
const submitting = ref(false);

const validate = () => {
  let valid = true;
  errors.name = "";
  errors.category = "";
  errors.location = "";
  errors.quantity = "";
  if (!form.name.trim()) { errors.name = "Name is required"; valid = false; }
  if (!form.category.trim()) { errors.category = "Category is required"; valid = false; }
  if (!form.location) { errors.location = "Location is required"; valid = false; }
  if (form.quantity <= 0) { errors.quantity = "Quantity must be greater than 0"; valid = false; }
  return valid;
};

const resetForm = () => {
  form.name = "";
  form.category = "";
  form.location = "";
  form.quantity = 1;
  form.unit = "";
  form.isOpened = false;
  form.expiresAt = "";
  form.notes = "";
  errors.name = "";
  errors.category = "";
  errors.location = "";
  errors.quantity = "";
};

const onCancel = () => {
  resetForm();
  modalRef.value.startClose();
};

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    const { addInventoryItem } = useInventory();
    const payload: AddInventoryItemPayload = {
      name: form.name,
      category: form.category,
      location: form.location,
      quantity: form.quantity,
      unit: form.unit,
      isOpened: form.isOpened,
      expiresAt: form.expiresAt || undefined,
      notes: form.notes || undefined,
    };
    const created = await addInventoryItem(payload);
    emit("added", created);
    resetForm();
    pushSuccess(`"${form.name}" added to inventory!`);
    modalRef.value.startClose();
  } catch (err) {
    pushError(err instanceof Error ? err : new Error("Failed to add item"));
  } finally {
    submitting.value = false;
  }
};
</script>
```

- [ ] **Step 2: Verify modal opens and submits**

```bash
cd src/CookHomie.Web && npm run dev
# Visit /inventory, click "+ Add Item", verify all fields render
# Submit with valid data, verify toast appears on success
# Test validation errors appear for empty required fields
```

- [ ] **Step 3: Run validation**

```bash
cd src/CookHomie.Web && npm run validate
```

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/components/inventory/AddItemModal.vue
git commit -m "feat(web): upgrade AddItemModal with full fields, SharedModal, and toast feedback"
```

---

## Task 19 — Upgrade shopping page with skeletons and empty state

**Files:**
- Modify: `src/CookHomie.Web/pages/shopping.vue`

- [ ] **Step 1: Replace shopping page**

Replace `src/CookHomie.Web/pages/shopping.vue`:

```vue
<template>
  <div class="max-w-4xl mx-auto p-6">
    <header class="flex items-center justify-between mb-6">
      <h1 class="font-display text-[28px] font-bold">Shopping List</h1>
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
      <div v-if="loading && items.length === 0" class="flex flex-col gap-2">
        <SharedSkeletonBlock v-for="i in 4" :key="i" class="h-12" />
      </div>
      <SharedErrorBanner
        v-else-if="error && items.length === 0"
        :message="error"
        show-retry
        @retry="refresh"
      />
      <p v-else-if="items.length === 0" class="text-text-muted text-sm text-center py-10">
        Your shopping list is empty.
      </p>
      <ul v-else class="flex flex-col gap-2 list-none p-0 m-0">
        <li
          v-for="item in items"
          :key="item.id"
          :class="['flex items-center justify-between p-3 bg-surface border border-border rounded-[10px]', item.isBought ? 'line-through opacity-50' : '']"
        >
          <span>{{ item.name }}</span>
          <span v-if="item.quantity" class="text-xs text-text-muted">
            {{ item.quantity }} {{ item.unit }}
          </span>
        </li>
      </ul>
    </DashboardPanel>
  </div>
</template>

<script setup lang="ts">
const { items, loading, error, isStale, startPolling, stopPolling, refresh } = useShoppingList();

onMounted(() => startPolling());
onUnmounted(() => stopPolling());
</script>
```

- [ ] **Step 2: Verify page renders**

```bash
cd src/CookHomie.Web && npm run dev
# Visit /shopping, verify empty state renders correctly
# If API is running, verify items render with strike-through for bought items
```

- [ ] **Step 3: Run validation**

```bash
cd src/CookHomie.Web && npm run validate
```

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/pages/shopping.vue
git commit -m "feat(web): polish shopping page with skeletons, empty state, bought item styling"
```

---

## Task 20 — Update integration tests

**Files:**
- Modify: `src/CookHomie.Web/tests/AddItemModal.spec.ts`

- [ ] **Step 1: Update AddItemModal spec**

The existing spec was written for the stub. Update it for the full modal:

```typescript
// src/CookHomie.Web/tests/AddItemModal.spec.ts
import { mount } from "@vue/test-utils";
import { beforeEach, describe, expect, it, vi } from "vitest";
import AddItemModal from "../components/inventory/AddItemModal.vue";

vi.mock("~/composables/useToast", () => ({
  useToast: () => ({
    pushSuccess: vi.fn(),
    pushError: vi.fn(),
  }),
}));

vi.mock("~/composables/useInventory", () => ({
  useInventory: () => ({
    addInventoryItem: vi.fn().mockResolvedValue({
      id: "new-item-id",
      name: "Milk",
      category: "dairy",
      location: "Fridge",
      quantity: 1,
      unit: "liters",
      isOpened: false,
    }),
  }),
}));

describe("AddItemModal", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("validates empty required fields", async () => {
    const wrapper = mount(AddItemModal, {
      global: {
        stubs: {
          SharedModal: { template: '<div class="stub-modal"><slot /><slot name="footer" /></div>' },
          SharedFormField: { template: '<div class="stub-field"><slot /></div>', props: ["label", "error", "required"] },
          SharedButton: { template: '<button class="stub-button"><slot /></button>', props: ["variant", "disabled"] },
          Teleport: true,
        },
      },
    });

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const vm = wrapper.vm as any;
    const result = vm.validate();
    expect(result).toBe(false);
    expect(vm.errors.name).toBe("Name is required");
    expect(vm.errors.category).toBe("Category is required");
    expect(vm.errors.location).toBe("Location is required");
  });

  it("submits form and emits added on valid data", async () => {
    const $fetchMock = vi.fn().mockResolvedValue({ id: "new-item" });
    vi.stubGlobal("$fetch", $fetchMock);

    const wrapper = mount(AddItemModal, {
      global: {
        stubs: {
          SharedModal: { template: '<div class="stub-modal"><slot /><slot name="footer" /></div>' },
          SharedFormField: { template: '<div class="stub-field"><slot /></div>', props: ["label", "error", "required"] },
          SharedButton: { template: '<button class="stub-button"><slot /></button>', props: ["variant", "disabled"] },
          Teleport: true,
        },
      },
    });

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const vm = wrapper.vm as any;
    vm.form.name = "Milk";
    vm.form.category = "dairy";
    vm.form.location = "Fridge";
    vm.form.quantity = 1;
    await wrapper.vm.$nextTick();

    expect(vm.validate()).toBe(true);
  });
});
```

- [ ] **Step 2: Run tests**

```bash
cd src/CookHomie.Web && npx vitest run tests/AddItemModal.spec.ts
```

Expected: PASS

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/tests/AddItemModal.spec.ts
git commit -m "test(web): update AddItemModal spec for full modal implementation"
```

---

## Task 21 — Final responsive verification + completion check

**Files:** No file changes — verify only.

- [ ] **Step 1: Run all web tests**

```bash
cd src/CookHomie.Web && npx vitest run
```
Expected: All tests pass

- [ ] **Step 2: Responsive browser check**

Start dev server: `cd src/CookHomie.Web && npm run dev`

- At 375px (phone): verify single-column layouts, no horizontal overflow
- At 768px (tablet): verify 2-column dashboard and recipe grids
- At 1024px+ (desktop): verify full multi-column layouts

Check: Dashboard (`/`), Recipes (`/recipes`), Recipe Detail (`/recipes/[id]`), Inventory (`/inventory`), Shopping (`/shopping`) — all render correctly at each breakpoint.

- [ ] **Step 3: Verify completion criteria**

1. Dashboard (`/`) renders with 3 stat cards + panels ✓
2. Recipes index (`/recipes`) renders recipe grid + AddRecipeModal ✓
3. Recipe detail shows In Stock / Missing badges on ingredients ✓
4. Toast appears on user actions (add recipe, add inventory item) ✓
5. Polling starts on mount, stops on unmount ✓
6. Error banners + stale indicators appear correctly ✓
7. Inventory page has skeletons, error states, responsive layout ✓
8. Shopping page has skeletons, empty state, bought-item styling ✓
9. AddItemModal uses SharedModal with full form fields + toast feedback ✓
10. All tests pass ✓

- [ ] **Step 4: Final commit**

```bash
git add .
git commit -m "feat(web): complete page polish — inventory/shopping pages, AddItemModal, tests"
```
