# Milestone 2 — Foundation Shell

> **For agentic workers:** Each step is `- [ ]`. Implement task-by-task on its own branch. Commit after each task.

**Goal:** Build the shared platform base — design tokens, polling composable, toast system, shared UI primitives, inventory polling upgrade, and TypeScript type additions.

**Tech Stack:** Nuxt 3 + Vue 3 + TypeScript + Vitest + CSS custom properties

---

## Task 1 — Calm Utility design tokens

**Files:**
- Create: `src/CookHomie.Web/assets/css/tokens.css`
- Modify: `src/CookHomie.Web/nuxt.config.ts`

- [ ] **Step 1: Create tokens.css**

```css
:root {
  /* Calm Utility palette */
  --color-bg: #fafaf8;
  --color-surface: #ffffff;
  --color-surface-hover: #f5f4f0;
  --color-border: #e8e6e1;
  --color-border-strong: #ccc9c3;
  --color-text-primary: #1a1918;
  --color-text-secondary: #6b6860;
  --color-text-muted: #9c9991;
  --color-accent: #c8602a;
  --color-accent-hover: #b5541f;
  --color-accent-subtle: #fdf0e9;
  --color-success: #3d7a4f;
  --color-success-subtle: #edf5f0;
  --color-warning: #a06723;
  --color-warning-subtle: #fff3e0;
  --color-error: #b83232;
  --color-error-subtle: #fef2f2;
  --color-info: #2563a8;
  --color-info-subtle: #eff6ff;

  /* Typography */
  --font-display: "Literata", Georgia, serif;
  --font-body: "DM Sans", system-ui, sans-serif;
  --font-mono: "DM Mono", monospace;

  /* Spacing scale */
  --space-1: 4px;
  --space-2: 8px;
  --space-3: 12px;
  --space-4: 16px;
  --space-5: 20px;
  --space-6: 24px;
  --space-8: 32px;
  --space-10: 40px;
  --space-12: 48px;

  /* Radius */
  --radius-sm: 6px;
  --radius-md: 10px;
  --radius-lg: 16px;
  --radius-full: 9999px;

  /* Shadows */
  --shadow-sm: 0 1px 3px rgba(26,25,24,0.06), 0 1px 2px rgba(26,25,24,0.04);
  --shadow-md: 0 4px 12px rgba(26,25,24,0.08), 0 2px 4px rgba(26,25,24,0.04);
  --shadow-lg: 0 8px 24px rgba(26,25,24,0.10), 0 4px 8px rgba(26,25,24,0.06);

  /* Transitions */
  --transition-fast: 120ms ease;
  --transition-base: 200ms ease;
  --transition-slow: 350ms ease;
}
```

- [ ] **Step 2: Update nuxt.config.ts**

Add to `defineNuxtConfig`:
```ts
css: ["~/assets/css/tokens.css"]
```

- [ ] **Step 3: Run dev server check**

Run: `cd src/CookHomie.Web && npm run dev`
Expected: Nuxt starts without CSS errors

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/assets/css/tokens.css src/CookHomie.Web/nuxt.config.ts
git commit -m "feat(web): add Calm Utility design tokens"
```

---

## Task 2 — usePollingFetch composable

**Files:**
- Create: `src/CookHomie.Web/composables/usePollingFetch.ts`
- Create: `src/CookHomie.Web/tests/usePollingFetch.spec.ts`

- [ ] **Step 1: Write failing test**

```typescript
// src/CookHomie.Web/tests/usePollingFetch.spec.ts
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { usePollingFetch } from "../composables/usePollingFetch";

describe("usePollingFetch", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));
    vi.stubGlobal("useNuxtApp", () => ({}));
  });

  it("fetches immediately on mount", async () => {
    const spy = vi.fn().mockResolvedValue({ data: "test" });
    vi.stubGlobal("$fetch", spy);

    const { data, start } = usePollingFetch("/api/test");
    await start();

    expect(spy).toHaveBeenCalledWith("/api/test");
    expect(data.value).toEqual({ data: "test" });
  });

  it("polls every 30 seconds", async () => {
    const spy = vi.fn().mockResolvedValue({ data: "value" });
    vi.stubGlobal("$fetch", spy);

    const { start } = usePollingFetch("/api/test");
    await start();

    spy.mockClear();
    await vi.advanceTimersByTimeAsync(30000);

    expect(spy).toHaveBeenCalledTimes(1);
  });

  it("stops polling on stop()", async () => {
    const spy = vi.fn().mockResolvedValue({ data: "value" });
    vi.stubGlobal("$fetch", spy);

    const { start, stop } = usePollingFetch("/api/test");
    await start();
    stop();

    spy.mockClear();
    await vi.advanceTimersByTimeAsync(35000);

    expect(spy).not.toHaveBeenCalled();
  });

  it("sets loading true during fetch, false after", async () => {
    vi.stubGlobal("$fetch", vi.fn().mockImplementation(() => new Promise(r => setTimeout(r, 100))));
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

    const { data, loading, start } = usePollingFetch("/api/test");
    const promise = start();

    expect(loading.value).toBe(true);
    await promise;
    expect(loading.value).toBe(false);
  });

  it("handles fetch errors and sets error state", async () => {
    const spy = vi.fn().mockRejectedValue(new Error("network error"));
    vi.stubGlobal("$fetch", spy);
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

    const { error, start } = usePollingFetch("/api/test");
    await start();

    expect(error.value).toBe("network error");
  });
});
```

- [ ] **Step 2: Run test — verify it fails**

Run: `cd src/CookHomie.Web && npx vitest run tests/usePollingFetch.spec.ts`
Expected: FAIL — composable not defined

- [ ] **Step 3: Write implementation**

```typescript
// src/CookHomie.Web/composables/usePollingFetch.ts
export const usePollingFetch = <T>(url: string, options?: {
  pollIntervalMs?: number;
  onSuccess?: (data: T) => void;
  onError?: (err: Error) => void;
}) => {
  const pollIntervalMs = options?.pollIntervalMs ?? 30000;

  const data = useState<T | null>(`poll-${url}`, () => null);
  const loading = useState<boolean>(`poll-loading-${url}`, () => false);
  const error = useState<string | null>(`poll-error-${url}`, () => null);
  const isStale = useState<boolean>(`poll-stale-${url}`, () => false);

  let intervalId: ReturnType<typeof setInterval> | null = null;

  const fetchData = async () => {
    if (loading.value) return;
    loading.value = true;
    error.value = null;

    try {
      const result = await $fetch<T>(url);
      data.value = result;
      isStale.value = false;
      options?.onSuccess?.(result);
    } catch (err) {
      const msg = err instanceof Error ? err.message : "Fetch failed";
      error.value = msg;
      isStale.value = data.value !== null;
      options?.onError?.(err instanceof Error ? err : new Error(msg));
    } finally {
      loading.value = false;
    }
  };

  const start = async () => {
    await fetchData();
    intervalId = setInterval(fetchData, pollIntervalMs);
  };

  const stop = () => {
    if (intervalId !== null) {
      clearInterval(intervalId);
      intervalId = null;
    }
  };

  const refresh = async () => {
    await fetchData();
  };

  return { data, loading, error, isStale, start, stop, refresh };
};
```

- [ ] **Step 4: Run test — verify it passes**

Run: `cd src/CookHomie.Web && npx vitest run tests/usePollingFetch.spec.ts`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/composables/usePollingFetch.ts src/CookHomie.Web/tests/usePollingFetch.spec.ts
git commit -m "feat(web): add usePollingFetch composable"
```

---

## Task 3 — useToast composable + ToastContainer

**Files:**
- Create: `src/CookHomie.Web/composables/useToast.ts`
- Create: `src/CookHomie.Web/components/shared/ToastContainer.vue`
- Create: `src/CookHomie.Web/tests/useToast.spec.ts`
- Modify: `src/CookHomie.Web/layouts/default.vue`

- [ ] **Step 1: Write failing test**

```typescript
// src/CookHomie.Web/tests/useToast.spec.ts
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { useToast } from "../composables/useToast";

describe("useToast", () => {
  beforeEach(() => {
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));
  });

  it("pushes a success toast", () => {
    const { toasts, pushSuccess } = useToast();
    pushSuccess("Item added");
    expect(toasts.value).toContainEqual(expect.objectContaining({ type: "success", message: "Item added" }));
  });

  it("pushes an error toast with parsed message", () => {
    const { toasts, pushError } = useToast();
    pushError(new Error("Validation failed: name is required"));
    expect(toasts.value).toContainEqual(expect.objectContaining({
      type: "error",
      message: expect.stringContaining("name is required")
    }));
  });

  it("clears all toasts", () => {
    const { toasts, pushSuccess, clear } = useToast();
    pushSuccess("One");
    clear();
    expect(toasts.value).toHaveLength(0);
  });

  it("removes a toast by id", () => {
    const { toasts, pushSuccess, remove } = useToast();
    pushSuccess("One");
    const id = toasts.value[0].id;
    remove(id);
    expect(toasts.value).toHaveLength(0);
  });
});
```

- [ ] **Step 2: Run test — verify it fails**

Run: `cd src/CookHomie.Web && npx vitest run tests/useToast.spec.ts`
Expected: FAIL — composable not defined

- [ ] **Step 3: Write useToast implementation**

```typescript
// src/CookHomie.Web/composables/useToast.ts
export type ToastType = "success" | "error" | "warning" | "info";

export interface Toast {
  id: string;
  type: ToastType;
  message: string;
  createdAt: number;
}

export const useToast = () => {
  const toasts = useState<Toast[]>("toasts", () => []);

  const push = (type: ToastType, message: string) => {
    const id = Math.random().toString(36).slice(2);
    toasts.value.push({ id, type, message, createdAt: Date.now() });
    setTimeout(() => remove(id), 5000);
  };

  const pushSuccess = (message: string) => push("success", message);
  const pushError = (err: Error | string) => {
    const msg = typeof err === "string" ? err : parseErrorMessage(err);
    push("error", msg);
  };
  const pushWarning = (message: string) => push("warning", message);
  const pushInfo = (message: string) => push("info", message);

  const remove = (id: string) => {
    const idx = toasts.value.findIndex(t => t.id === id);
    if (idx !== -1) toasts.value.splice(idx, 1);
  };

  const clear = () => { toasts.value = []; };

  return { toasts, pushSuccess, pushError, pushWarning, pushInfo, remove, clear };
};

function parseErrorMessage(err: Error): string {
  const msg = err.message;
  const colonIdx = msg.indexOf(":");
  if (colonIdx !== -1 && colonIdx < msg.length - 1) {
    return msg.slice(colonIdx + 1).trim();
  }
  return msg;
}
```

- [ ] **Step 4: Write ToastContainer component**

```vue
<!-- src/CookHomie.Web/components/shared/ToastContainer.vue -->
<template>
  <Teleport to="body">
    <div class="toast-container" aria-live="polite">
      <TransitionGroup name="toast">
        <div
          v-for="toast in toasts"
          :key="toast.id"
          :class="['toast', `toast--${toast.type}`]"
          @click="remove(toast.id)"
        >
          <span class="toast__icon">{{ toastIcon(toast.type) }}</span>
          <span class="toast__message">{{ toast.message }}</span>
          <button class="toast__close" aria-label="Dismiss">×</button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { useToast } from "~/composables/useToast";
const { toasts, remove } = useToast();

const toastIcon = (type: string) => {
  if (type === "success") return "✓";
  if (type === "error") return "✕";
  if (type === "warning") return "⚠";
  return "ℹ";
};
</script>

<style scoped>
.toast-container {
  position: fixed;
  bottom: var(--space-6);
  right: var(--space-6);
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
  pointer-events: none;
}

.toast {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  background: var(--color-surface);
  box-shadow: var(--shadow-lg);
  border-left: 4px solid;
  cursor: pointer;
  pointer-events: all;
  max-width: 360px;
  font-family: var(--font-body);
  font-size: 14px;
}

.toast--success { border-color: var(--color-success); }
.toast--error { border-color: var(--color-error); }
.toast--warning { border-color: var(--color-warning); }
.toast--info { border-color: var(--color-info); }

.toast__icon { font-size: 16px; }
.toast__message { flex: 1; color: var(--color-text-primary); }
.toast__close { background: none; border: none; cursor: pointer; font-size: 18px; color: var(--color-text-muted); }

.toast-enter-active, .toast-leave-active { transition: all var(--transition-base); }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateX(20px); }
</style>
```

- [ ] **Step 5: Mount ToastContainer in default layout**

Modify `src/CookHomie.Web/layouts/default.vue` to add `<ToastContainer />` inside or after `<main>`.

- [ ] **Step 6: Run test — verify it passes**

Run: `cd src/CookHomie.Web && npx vitest run tests/useToast.spec.ts`
Expected: PASS

- [ ] **Step 7: Commit**

```bash
git add src/CookHomie.Web/composables/useToast.ts src/CookHomie.Web/components/shared/ToastContainer.vue src/CookHomie.Web/tests/useToast.spec.ts src/CookHomie.Web/layouts/default.vue
git commit -m "feat(web): add toast system with useToast composable"
```

---

## Task 4 — Shared component primitives

**Files:**
- Create: `src/CookHomie.Web/components/shared/SkeletonBlock.vue`
- Create: `src/CookHomie.Web/components/shared/ErrorBanner.vue`
- Create: `src/CookHomie.Web/components/shared/StaleIndicator.vue`

- [ ] **Step 1: Write SkeletonBlock**

```vue
<!-- src/CookHomie.Web/components/shared/SkeletonBlock.vue -->
<template>
  <div
    class="skeleton"
    :style="{ width: width, height: height, borderRadius: radius }"
    aria-hidden="true"
  />
</template>

<script setup lang="ts">
defineProps<{
  width?: string;
  height?: string;
  radius?: string;
}>();
</script>

<style scoped>
.skeleton {
  background: linear-gradient(90deg, var(--color-border) 25%, var(--color-surface-hover) 50%, var(--color-border) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s ease infinite;
  border-radius: var(--radius-md);
}

@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
</style>
```

- [ ] **Step 2: Write ErrorBanner**

```vue
<!-- src/CookHomie.Web/components/shared/ErrorBanner.vue -->
<template>
  <div class="error-banner" role="alert">
    <span class="error-banner__icon">⚠</span>
    <span class="error-banner__message">{{ message }}</span>
    <button v-if="showRetry" class="error-banner__retry" @click="$emit('retry')">Retry</button>
  </div>
</template>

<script setup lang="ts">
defineProps<{ message: string; showRetry?: boolean }>();
defineEmits(["retry"]);
</script>

<style scoped>
.error-banner {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-3) var(--space-4);
  background: var(--color-error-subtle);
  border: 1px solid var(--color-error);
  border-radius: var(--radius-md);
  color: var(--color-error);
  font-family: var(--font-body);
  font-size: 14px;
}

.error-banner__retry {
  margin-left: auto;
  background: none;
  border: 1px solid currentColor;
  border-radius: var(--radius-sm);
  color: inherit;
  padding: var(--space-1) var(--space-3);
  cursor: pointer;
  font-size: 13px;
}
</style>
```

- [ ] **Step 3: Write StaleIndicator**

```vue
<!-- src/CookHomie.Web/components/shared/StaleIndicator.vue -->
<template>
  <div class="stale-indicator">
    <span>⚡</span>
    <span>Data may be outdated</span>
    <button @click="$emit('refresh')">Refresh</button>
  </div>
</template>

<script setup lang="ts">
defineEmits(["refresh"]);
</script>

<style scoped>
.stale-indicator {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-2) var(--space-3);
  background: var(--color-warning-subtle);
  border-radius: var(--radius-sm);
  font-size: 12px;
  color: var(--color-warning);
}

.stale-indicator button {
  background: none;
  border: none;
  text-decoration: underline;
  cursor: pointer;
  color: inherit;
  font-size: inherit;
}
</style>
```

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/components/shared/SkeletonBlock.vue src/CookHomie.Web/components/shared/ErrorBanner.vue src/CookHomie.Web/components/shared/StaleIndicator.vue
git commit -m "feat(web): add shared component primitives (SkeletonBlock, ErrorBanner, StaleIndicator)"
```

---

## Task 5 — Upgrade useInventory with polling + manual refresh

**Files:**
- Modify: `src/CookHomie.Web/composables/useInventory.ts`
- Modify: `src/CookHomie.Web/tests/useInventory.spec.ts`

- [ ] **Step 1: Read existing implementation**

Review `src/CookHomie.Web/composables/useInventory.ts`

- [ ] **Step 2: Add polling tests to useInventory.spec.ts**

Add these tests to the existing test file:

```typescript
it("starts polling on start() and stops on stop()", async () => {
  vi.useFakeTimers();
  const fetchSpy = vi.fn().mockResolvedValue([{ id: "1", name: "Milk", category: "dairy", location: "Fridge", quantity: 1, unit: "liter", isOpened: false }]);
  vi.stubGlobal("$fetch", fetchSpy);
  vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

  const { startPolling, stopPolling } = useInventory();
  await startPolling();

  fetchSpy.mockClear();
  await vi.advanceTimersByTimeAsync(30000);
  expect(fetchSpy).toHaveBeenCalled();
  stopPolling();

  fetchSpy.mockClear();
  await vi.advanceTimersByTimeAsync(35000);
  expect(fetchSpy).not.toHaveBeenCalled();
  vi.useRealTimers();
});

it("refresh() forces a manual fetch", async () => {
  const fetchSpy = vi.fn().mockResolvedValue([{ id: "2", name: "Eggs", category: "dairy", location: "Fridge", quantity: 12, unit: "units", isOpened: false }]);
  vi.stubGlobal("$fetch", fetchSpy);
  vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

  const { refresh } = useInventory();
  await refresh();
  expect(fetchSpy).toHaveBeenCalledTimes(1);
});
```

- [ ] **Step 3: Run test — verify it fails**

Run: `cd src/CookHomie.Web && npx vitest run tests/useInventory.spec.ts`
Expected: FAIL — startPolling/stopPolling/refresh not in return

- [ ] **Step 4: Update useInventory**

Replace `src/CookHomie.Web/composables/useInventory.ts` with:

```typescript
import type { AddInventoryItemPayload, InventoryItem } from "../types";

export const useInventory = () => {
  const items = useState<InventoryItem[]>("inventory-items", () => []);
  const loading = useState<boolean>("inventory-loading", () => false);
  const error = useState<string | null>("inventory-error", () => null);
  const isStale = useState<boolean>("inventory-stale", () => false);

  const fetchItems = async () => {
    loading.value = true;
    error.value = null;
    try {
      const result = await $fetch<InventoryItem[]>("/api/inventory");
      items.value = result;
      isStale.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load inventory";
      isStale.value = items.value.length > 0;
      throw err;
    } finally {
      loading.value = false;
    }
  };

  let intervalId: ReturnType<typeof setInterval> | null = null;

  const startPolling = () => {
    fetchItems();
    intervalId = setInterval(fetchItems, 30000);
  };

  const stopPolling = () => {
    if (intervalId !== null) {
      clearInterval(intervalId);
      intervalId = null;
    }
  };

  const refresh = async () => { await fetchItems(); };

  const loadInventory = async () => { await fetchItems(); };

  const addInventoryItem = async (payload: AddInventoryItemPayload): Promise<InventoryItem> => {
    loading.value = true;
    error.value = null;
    try {
      const created = await $fetch<InventoryItem>("/api/inventory", { method: "POST", body: payload });
      items.value.unshift(created);
      return created;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add inventory item";
      throw err;
    } finally {
      loading.value = false;
    }
  };

  return {
    items, loading, error, isStale,
    loadInventory, startPolling, stopPolling, refresh, addInventoryItem
  };
};
```

- [ ] **Step 5: Run tests — verify they pass**

Run: `cd src/CookHomie.Web && npx vitest run tests/useInventory.spec.ts`
Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add src/CookHomie.Web/composables/useInventory.ts src/CookHomie.Web/tests/useInventory.spec.ts
git commit -m "feat(web): add polling lifecycle to useInventory composable"
```

---

## Task 6 — Add Recipe types to TypeScript types file

**Files:**
- Modify: `src/CookHomie.Web/types/index.ts`
- Create: `src/CookHomie.Web/tests/recipe-types.spec.ts`

- [ ] **Step 1: Write failing type test**

```typescript
// src/CookHomie.Web/tests/recipe-types.spec.ts
import { describe, expect, it } from "vitest";
import type { Recipe, RecipeIngredient, DashboardSummary, AddRecipePayload } from "../types";

describe("Recipe types", () => {
  it("Recipe has required fields", () => {
    const r: Recipe = { id: "1", name: "Test", instructions: "Step 1", prepMinutes: 10, cookMinutes: 20, tags: ["quick"] };
    expect(r.name).toBe("Test");
    expect(r.prepMinutes).toBe(10);
  });

  it("AddRecipePayload omits id", () => {
    const p: AddRecipePayload = { name: "New", instructions: "Do", prepMinutes: 5, cookMinutes: 10, tags: [] };
    expect("id" in p).toBe(false);
  });
});
```

- [ ] **Step 2: Run test — verify it fails**

Run: `cd src/CookHomie.Web && npx vitest run tests/recipe-types.spec.ts`
Expected: FAIL — types not defined

- [ ] **Step 3: Add new types to types/index.ts**

Append to `src/CookHomie.Web/types/index.ts`:

```typescript
export interface RecipeIngredient {
  name: string;
  quantity?: number;
  unit?: string;
  isInStock?: boolean;
}

export interface DashboardSummary {
  expiringCount: number;
  recipeMatchCount: number;
  shoppingCount: number;
}

export type AddRecipePayload = Omit<Recipe, "id">;
```

- [ ] **Step 4: Run test — verify it passes**

Run: `cd src/CookHomie.Web && npx vitest run tests/recipe-types.spec.ts`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/types/index.ts src/CookHomie.Web/tests/recipe-types.spec.ts
git commit -m "feat(web): add Recipe, RecipeIngredient, DashboardSummary types"
```
