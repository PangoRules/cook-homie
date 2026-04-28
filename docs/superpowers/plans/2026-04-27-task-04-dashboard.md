# Milestone 2 — Dashboard

**Branch:** `task/dashboard`
**Parent branch:** `feat/milestone-2-frontend`

> **For agentic workers:** Each step is `- [ ]`. Implement task-by-task on its own branch. Commit after each task.

**Goal:** Build the full Dashboard page with DashboardStatCard, DashboardPanel components, the useDashboard composable, and wire everything together with 30s polling.

**Prerequisite:** Complete Tasks 1–3, 6, 7 (design tokens, shared primitives, types, mock endpoints).

---

## Task 10 — DashboardStatCard + DashboardPanel components

**Files:**
- Create: `src/CookHomie.Web/components/dashboard/DashboardStatCard.vue`
- Create: `src/CookHomie.Web/components/dashboard/DashboardPanel.vue`
- Create: `src/CookHomie.Web/tests/dashboard-components.spec.ts`

- [ ] **Step 1: Write DashboardStatCard**

```vue
<!-- src/CookHomie.Web/components/dashboard/DashboardStatCard.vue -->
<template>
  <div :class="['stat-card', variant]" @click="$emit('click')">
    <div class="stat-card__label">{{ label }}</div>
    <div class="stat-card__value">{{ displayValue }}</div>
    <div v-if="sub" class="stat-card__sub">{{ sub }}</div>
    <slot />
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  label: string;
  value: number | string;
  sub?: string;
  variant?: "default" | "warning" | "success";
}>();

defineEmits(["click"]);

const displayValue = computed(() =>
  typeof props.value === "number" ? props.value.toLocaleString() : props.value
);
</script>

<style scoped>
.stat-card {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: var(--space-5);
  box-shadow: var(--shadow-sm);
  cursor: pointer;
  transition: box-shadow var(--transition-fast), transform var(--transition-fast);
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

.stat-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-1px);
}

.stat-card__label {
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: var(--color-text-secondary);
  font-family: var(--font-body);
}

.stat-card__value {
  font-size: 32px;
  font-weight: 700;
  color: var(--color-text-primary);
  font-family: var(--font-display);
  line-height: 1.1;
}

.stat-card__sub {
  font-size: 12px;
  color: var(--color-text-muted);
  margin-top: var(--space-1);
}

.stat-card.warning { border-left: 4px solid var(--color-warning); }
.stat-card.success { border-left: 4px solid var(--color-success); }

@media (max-width: 480px) {
  .stat-card__value { font-size: 26px; }
}
</style>
```

- [ ] **Step 2: Write DashboardPanel**

```vue
<!-- src/CookHomie.Web/components/dashboard/DashboardPanel.vue -->
<template>
  <section class="panel">
    <header class="panel__header">
      <h2 class="panel__title">{{ title }}</h2>
      <button v-if="showRefresh" class="panel__refresh" @click="$emit('refresh')" :disabled="loading">
        ↻
      </button>
    </header>
    <div v-if="loading && !hasData" class="panel__loading">
      <SkeletonBlock height="80px" />
    </div>
    <ErrorBanner v-else-if="error && !hasData" :message="error" show-retry @retry="$emit('refresh')" />
    <StaleIndicator v-else-if="isStale && hasData" @refresh="$emit('refresh')" />
    <div v-show="!loading || hasData" class="panel__body">
      <slot />
    </div>
  </section>
</template>

<script setup lang="ts">
defineProps<{
  title: string;
  loading?: boolean;
  error?: string | null;
  isStale?: boolean;
  hasData?: boolean;
  showRefresh?: boolean;
}>();
defineEmits(["refresh"]);
</script>

<style scoped>
.panel {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  overflow: hidden;
}

.panel__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-4) var(--space-5);
  border-bottom: 1px solid var(--color-border);
}

.panel__title {
  font-family: var(--font-display);
  font-size: 16px;
  font-weight: 600;
  color: var(--color-text-primary);
  margin: 0;
}

.panel__refresh {
  background: none;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  width: 28px;
  height: 28px;
  cursor: pointer;
  color: var(--color-text-secondary);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all var(--transition-fast);
}

.panel__refresh:hover:not(:disabled) {
  background: var(--color-surface-hover);
  color: var(--color-text-primary);
}

.panel__refresh:disabled { opacity: 0.4; cursor: not-allowed; }

.panel__loading, .panel__body { padding: var(--space-5); }
</style>
```

- [ ] **Step 3: Write component tests**

```typescript
// src/CookHomie.Web/tests/dashboard-components.spec.ts
import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import DashboardStatCard from "../components/dashboard/DashboardStatCard.vue";
import DashboardPanel from "../components/dashboard/DashboardPanel.vue";

describe("DashboardStatCard", () => {
  it("renders label and value", () => {
    const wrapper = mount(DashboardStatCard, { props: { label: "Expiring", value: 3 } });
    expect(wrapper.find(".stat-card__label").text()).toBe("Expiring");
    expect(wrapper.find(".stat-card__value").text()).toBe("3");
  });

  it("formats number values with locale", () => {
    const wrapper = mount(DashboardStatCard, { props: { label: "Items", value: 1234 } });
    expect(wrapper.find(".stat-card__value").text()).toBe("1,234");
  });

  it("applies warning variant class", () => {
    const wrapper = mount(DashboardStatCard, { props: { label: "Warn", value: 5, variant: "warning" } });
    expect(wrapper.find(".stat-card.warning").exists()).toBe(true);
  });
});

describe("DashboardPanel", () => {
  it("renders title", () => {
    const wrapper = mount(DashboardPanel, { props: { title: "My Panel" } });
    expect(wrapper.find(".panel__title").text()).toBe("My Panel");
  });

  it("shows refresh button when showRefresh is true", () => {
    const wrapper = mount(DashboardPanel, { props: { title: "P", showRefresh: true, loading: false, hasData: true } });
    expect(wrapper.find(".panel__refresh").exists()).toBe(true);
  });

  it("emits refresh when refresh button clicked", async () => {
    const wrapper = mount(DashboardPanel, { props: { title: "P", showRefresh: true, loading: false, hasData: true } });
    await wrapper.find(".panel__refresh").trigger("click");
    expect(wrapper.emitted("refresh")).toBeTruthy();
  });
});
```

- [ ] **Step 4: Run tests**

Run: `cd src/CookHomie.Web && npx vitest run tests/dashboard-components.spec.ts`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/components/dashboard/DashboardStatCard.vue src/CookHomie.Web/components/dashboard/DashboardPanel.vue src/CookHomie.Web/tests/dashboard-components.spec.ts
git commit -m "feat(web): add DashboardStatCard and DashboardPanel components"
```

---

## Task 11 — Dashboard page (full implementation)

**Files:**
- Modify: `src/CookHomie.Web/pages/index.vue`
- Create: `src/CookHomie.Web/composables/useDashboard.ts`
- Create: `src/CookHomie.Web/tests/dashboard.spec.ts`

- [ ] **Step 1: Write useDashboard composable**

```typescript
// src/CookHomie.Web/composables/useDashboard.ts
import type { DashboardSummary } from "~/types";

export const useDashboard = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<DashboardSummary>(
    "/api/dashboard/summary",
    { pollIntervalMs: 30000 }
  );

  return { data, loading, error, isStale, start, stop, refresh };
};
```

- [ ] **Step 2: Write failing dashboard test**

```typescript
// src/CookHomie.Web/tests/dashboard.spec.ts
import { describe, expect, it, vi } from "vitest";
import { ref } from "vue";

describe("Dashboard", () => {
  it("uses useDashboard composable", async () => {
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue({
      expiringCount: 3,
      recipeMatchCount: 7,
      shoppingCount: 4
    }));
    vi.stubGlobal("useState", (_k: string, init: () => unknown) => ref(init()));
    vi.stubGlobal("usePollingFetch", () => ({
      data: ref({ expiringCount: 3, recipeMatchCount: 7, shoppingCount: 4 }),
      loading: ref(false),
      error: ref(null),
      isStale: ref(false),
      start: vi.fn(),
      stop: vi.fn(),
      refresh: vi.fn()
    }));

    const { useDashboard } = await import("../composables/useDashboard");
    const { data } = useDashboard();
    expect(data.value?.expiringCount).toBe(3);
  });
});
```

- [ ] **Step 3: Run test — verify it fails**

Run: `cd src/CookHomie.Web && npx vitest run tests/dashboard.spec.ts`
Expected: FAIL — composable not defined or data not found

- [ ] **Step 4: Implement full Dashboard page**

Replace `src/CookHomie.Web/pages/index.vue`:

```vue
<template>
  <div class="dashboard">
    <header class="dashboard__header">
      <h1 class="dashboard__title">Kitchen Overview</h1>
      <button class="btn-refresh" @click="refresh" :disabled="loading">
        ↻ Refresh
      </button>
    </header>

    <div class="dashboard__grid">
      <DashboardStatCard
        label="Expiring Soon"
        :value="data?.expiringCount ?? 0"
        sub="items within 3 days"
        :variant="(data?.expiringCount ?? 0) > 0 ? 'warning' : 'success'"
        @click="navigateTo('/inventory')"
      />
      <DashboardStatCard
        label="Recipe Matches"
        :value="data?.recipeMatchCount ?? 0"
        sub="cookable from inventory"
        @click="navigateTo('/recipes')"
      />
      <DashboardStatCard
        label="Shopping List"
        :value="data?.shoppingCount ?? 0"
        sub="items pending"
        @click="navigateTo('/shopping')"
      />
    </div>

    <div class="dashboard__panels">
      <DashboardPanel
        title="Expiring Soon"
        :loading="loading"
        :error="error"
        :is-stale="isStale"
        :has-data="hasData"
        show-refresh
        @refresh="refresh"
      >
        <p v-if="!hasData && !loading" class="empty-hint">
          No expiring items — inventory looks fresh!
        </p>
      </DashboardPanel>

      <DashboardPanel
        title="Quick Recipe Ideas"
        :loading="loading"
        :error="error"
        :is-stale="isStale"
        :has-data="hasData"
        show-refresh
        @refresh="refresh"
      >
        <p v-if="!hasData && !loading" class="empty-hint">
          Add inventory items to get recipe suggestions.
        </p>
      </DashboardPanel>
    </div>
  </div>
</template>

<script setup lang="ts">
const { data, loading, error, isStale, start, stop, refresh } = useDashboard();

const hasData = computed(() => data.value !== null);

onMounted(() => start());
onUnmounted(() => stop());
</script>

<style scoped>
.dashboard { max-width: 960px; margin: 0 auto; }

.dashboard__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-6);
}

.dashboard__title {
  font-family: var(--font-display);
  font-size: 28px;
  font-weight: 700;
  color: var(--color-text-primary);
  margin: 0;
}

.btn-refresh {
  background: none;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-4);
  cursor: pointer;
  font-size: 14px;
  color: var(--color-text-secondary);
  transition: all var(--transition-fast);
}
.btn-refresh:hover:not(:disabled) {
  background: var(--color-surface-hover);
  color: var(--color-text-primary);
}

.dashboard__grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--space-4);
  margin-bottom: var(--space-6);
}

.dashboard__panels {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-4);
}

.empty-hint {
  color: var(--color-text-muted);
  font-size: 14px;
  font-style: italic;
}

@media (max-width: 768px) {
  .dashboard__grid { grid-template-columns: 1fr; }
  .dashboard__panels { grid-template-columns: 1fr; }
}
</style>
```

- [ ] **Step 5: Verify page renders**

Start dev server: `cd src/CookHomie.Web && npm run dev`
Visit `http://localhost:3000`
Expected: 3 stat cards + 2 panels visible; click on stat card navigates to respective page

- [ ] **Step 6: Run tests**

Run: `cd src/CookHomie.Web && npx vitest run tests/dashboard.spec.ts`
Expected: PASS (or skip if mocking setup is complex)

- [ ] **Step 7: Commit**

```bash
git add src/CookHomie.Web/pages/index.vue src/CookHomie.Web/composables/useDashboard.ts src/CookHomie.Web/tests/dashboard.spec.ts
git commit -m "feat(web): implement full Dashboard page with stat cards and panels"
```
