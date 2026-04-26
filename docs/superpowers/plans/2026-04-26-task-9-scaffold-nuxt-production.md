# Task 9: Scaffold Nuxt Production Structure Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Scaffold Nuxt 3 production structure with layouts, pages, components, composables, and types for CookHomie Web

**Architecture:** Create file structure following Nuxt 3 conventions. Pages for inventory/shopping/recipes, composables for API interaction, types for TypeScript definitions. No auth or complex state - static pages with basic fetch hydration.

**Tech Stack:** Nuxt 3 + Vue 3 + Vitest + TypeScript

---

### Task 9.1: Create types/index.ts

**Files:**
- Create: `src/CookHomie.Web/types/index.ts`
- Test: (no test for types)

- [ ] **Step 1: Create types directory and TypeScript definitions**

Run: `mkdir -p src/CookHomie.Web/types`

```typescript
// src/CookHomie.Web/types/index.ts
export interface InventoryItem {
  id: string;
  name: string;
  category: string;
  location: string;
  quantity: number;
  unit: string;
  expiresAt?: string;
  isOpened: boolean;
  notes?: string;
}

export interface ShoppingItem {
  id: string;
  name: string;
  quantity?: number;
  unit?: string;
  priority: 'low' | 'medium' | 'high';
  isBought: boolean;
  linkedRecipeId?: string;
}

export interface Recipe {
  id: string;
  name: string;
  instructions: string;
  prepMinutes: number;
  cookMinutes: number;
  tags: string[];
  source?: string;
}

export type AddInventoryItemPayload = Omit<InventoryItem, 'id'>;
export type AddShoppingItemPayload = Omit<ShoppingItem, 'id'>;
```

- [ ] **Step 2: Commit**

```bash
git add src/CookHomie.Web/types
git commit -m "feat: add TypeScript types for inventory shopping recipes"
```

---

### Task 9.2: Create composables useInventory

**Files:**
- Create: `src/CookHomie.Web/composables/useInventory.ts`
- Test: `src/CookHomie.Web/tests/useInventory.spec.ts`

- [ ] **Step 1: Write failing composable test**

Run: `mkdir -p src/CookHomie.Web/composables src/CookHomie.Web/tests`

```typescript
// tests/useInventory.spec.ts
import { describe, it, expect, vi, beforeEach } from "vitest";
import { useInventory } from "../composables/useInventory";

describe("useInventory", () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it("loads inventory items from /api/inventory", async () => {
    const mockItems = [
      { id: "1", name: "Milk", category: "dairy", location: "Fridge", quantity: 1, unit: "liter", isOpened: false }
    ];
    
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue(mockItems) as any);
    
    const { items, loadInventory } = useInventory();
    await loadInventory();
    
    expect(items.value.length).toBe(1);
    expect(items.value[0].name).toBe("Milk");
  });

  it("adds item and updates local state", async () => {
    const mockNewItem = { id: "2", name: "Eggs", category: "dairy", location: "Fridge", quantity: 12, unit: "units", isOpened: false };
    
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue(mockNewItem) as any);
    
    const { items, addInventoryItem } = useInventory();
    const result = await addInventoryItem({ name: "Eggs", category: "dairy", location: "Fridge", quantity: 12, unit: "units", isOpened: false });
    
    expect(result.name).toBe("Eggs");
    expect(items.value.length).toBe(1);
  });
});
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/CookHomie.Web && npm test -- useInventory.spec.ts`
Expected: FAIL because composable doesn't exist

- [ ] **Step 3: Create useInventory composable**

```typescript
// composables/useInventory.ts
import type { InventoryItem, AddInventoryItemPayload } from "~/types";

export const useInventory = () => {
  const items = useState<InventoryItem[]>("inventory-items", () => []);

  const loadInventory = async () => {
    items.value = await $fetch<InventoryItem[]>("/api/inventory");
  };

  const addInventoryItem = async (payload: AddInventoryItemPayload): Promise<InventoryItem> => {
    const created = await $fetch<InventoryItem>("/api/inventory", {
      method: "POST",
      body: payload,
    });
    items.value.unshift(created);
    return created;
  };

  return { items, loadInventory, addInventoryItem };
};
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd src/CookHomie.Web && npm test -- useInventory.spec.ts`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/composables src/CookHomie.Web/tests/useInventory.spec.ts
git commit -m "feat: add useInventory composable with tests"
```

---

### Task 9.3: Create default layout and inventory page

**Files:**
- Create: `src/CookHomie.Web/layouts/default.vue`
- Create: `src/CookHomie.Web/pages/inventory.vue`
- Test: (test component in later task)

- [ ] **Step 1: Create layouts directory**

Run: `mkdir -p src/CookHomie.Web/layouts`

```vue
<!-- layouts/default.vue -->
<template>
  <div class="app-layout">
    <header class="header">
      <nav>
        <NuxtLink to="/">Home</NuxtLink>
        <NuxtLink to="/inventory">Inventory</NuxtLink>
        <NuxtLink to="/recipes">Recipes</NuxtLink>
        <NuxtLink to="/shopping">Shopping</NuxtLink>
      </nav>
    </header>
    <main class="main">
      <slot />
    </main>
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.header {
  padding: 1rem;
  background: #f5f5f5;
  border-bottom: 1px solid #e0e0e0;
}

.header nav {
  display: flex;
  gap: 1rem;
}

.main {
  flex: 1;
  padding: 1rem;
}
</style>
```

- [ ] **Step 2: Create inventory page**

```vue
<!-- pages/inventory.vue -->
<template>
  <div class="inventory-page">
    <h1>Inventory</h1>
    <button @click="loadInventory">Refresh</button>
    <div v-if="items.length === 0">No items</div>
    <ul v-else>
      <li v-for="item in items" :key="item.id">
        {{ item.name }} - {{ item.quantity }} {{ item.unit }}
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
const { items, loadInventory } = useInventory();

await loadInventory();
</script>
```

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/layouts src/CookHomie.Web/pages/inventory.vue
git commit -m "feat: add default layout and inventory page"
```

---

### Task 9.4: Create shopping and recipes pages

**Files:**
- Create: `src/CookHomie.Web/pages/shopping.vue`
- Create: `src/CookHomie.Web/pages/recipes/index.vue`
- Create: `src/CookHomie.Web/pages/recipes/[id].vue`

- [ ] **Step 1: Create shopping page**

```vue
<!-- pages/shopping.vue -->
<template>
  <div class="shopping-page">
    <h1>Shopping List</h1>
    <p>Shopping list coming soon</p>
  </div>
</template>
```

- [ ] **Step 2: Create recipes index page**

```vue
<!-- pages/recipes/index.vue -->
<template>
  <div class="recipes-page">
    <h1>Recipes</h1>
    <p>Recipes coming soon</p>
  </div>
</template>
```

- [ ] **Step 3: Create recipe detail page**

```vue
<!-- pages/recipes/[id].vue -->
<template>
  <div class="recipe-detail">
    <h1>Recipe Detail</h1>
    <p>Recipe {{ $route.params.id }}</p>
  </div>
</template>

<script setup lang="ts">
// Placeholder for recipe detail
</script>
```

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/pages/shopping.vue src/CookHomie.Web/pages/recipes
git commit -m "feat: add shopping and recipes pages"
```

---

### Task 9.5: Add API proxy endpoints for inventory

**Files:**
- Create: `src/CookHomie.Web/server/api/inventory/index.get.ts`
- Create: `src/CookHomie.Web/server/api/inventory/index.post.ts`

- [ ] **Step 1: Create GET proxy endpoint**

```typescript
// server/api/inventory/index.get.ts
export default defineEventHandler(async (event) => {
  const apiBase = process.env.API_BASE_URL || "http://api:5000";
  return await $fetch(`${apiBase}/api/inventory`);
});
```

- [ ] **Step 2: Create POST proxy endpoint**

```typescript
// server/api/inventory/index.post.ts
export default defineEventHandler(async (event) => {
  const body = await readBody(event);
  const apiBase = process.env.API_BASE_URL || "http://api:5000";
  return await $fetch(`${apiBase}/api/inventory`, {
    method: "POST",
    body,
  });
});
```

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/server/api/inventory
git commit -m "feat: add inventory proxy endpoints"
```

---

### Task 9.6: Run full test verification

- [ ] **Step 1: Run all tests**

Run: `cd src/CookHomie.Web && npm test`
Expected: PASS

- [ ] **Step 2: Commit Task 9 completion**

```bash
git add src/CookHomie.Web
git commit -m "feat: complete task 9 - scaffold nuxt production structure"
```