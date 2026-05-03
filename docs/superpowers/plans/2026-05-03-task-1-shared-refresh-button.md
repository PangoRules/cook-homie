# Task 1: Shared Refresh Button
**Branch:** `task/task-1-shared-refresh-button`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Create: `src/CookHomie.Web/components/shared/RefreshButton.vue`
- Modify: `src/CookHomie.Web/pages/index.vue:5-11`
- Modify: `src/CookHomie.Web/components/dashboard/DashboardPanel.vue:5-12`

## Steps

- [ ] **Step 1: Create `RefreshButton.vue`**

Create `src/CookHomie.Web/components/shared/RefreshButton.vue`:

```vue
<template>
  <button
    class="bg-transparent border border-border rounded-md px-4 py-2 cursor-pointer text-sm text-text-secondary transition-all hover:bg-surface-hover hover:text-text-primary disabled:opacity-50 disabled:cursor-not-allowed"
    :disabled="loading"
    @click="$emit('refresh')"
  >
    ↻ {{ label }}
  </button>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  loading?: boolean;
  label?: string;
}>(), {
  loading: false,
  label: "Refresh",
});
defineEmits(["refresh"]);
</script>
```

- [ ] **Step 2: Update `pages/index.vue`**

In `src/CookHomie.Web/pages/index.vue`, replace lines 5–11 (the header refresh button) with:

```vue
<RefreshButton :loading="loading" @refresh="refresh" />
```

- [ ] **Step 3: Update `DashboardPanel.vue`**

In `src/CookHomie.Web/components/dashboard/DashboardPanel.vue`, replace lines 5–12 with:

```vue
<RefreshButton v-if="showRefresh" :loading="loading" @click="$emit('refresh')" />
```

Keep the `v-if="showRefresh"` on the parent — condition stays at call site per spec decision.

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/components/shared/RefreshButton.vue src/CookHomie.Web/pages/index.vue src/CookHomie.Web/components/dashboard/DashboardPanel.vue
git commit -m "feat(web): extract shared RefreshButton component"
```
