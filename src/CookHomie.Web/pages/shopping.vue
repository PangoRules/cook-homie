<template>
  <div class="p-6">
    <header class="flex items-center justify-between mb-6">
      <h1 class="font-display text-[28px] font-bold">Shopping List</h1>
    </header>

    <DashboardPanel
      title=""
      :loading="loading"
      :error="error"
      :is-stale="isStale"
      :has-data="localItems.length > 0"
      show-refresh
      @refresh="refresh"
    >
      <div v-if="loading && localItems.length === 0" class="flex flex-col gap-2">
        <SharedSkeletonBlock v-for="i in 4" :key="i" class="h-12" />
      </div>
      <SharedErrorBanner
        v-else-if="error && localItems.length === 0"
        :message="error"
        show-retry
        @retry="refresh"
      />
      <p v-else-if="localItems.length === 0" class="text-text-muted text-sm text-center py-10">
        Your shopping list is empty.
      </p>
      <ul v-else class="flex flex-col gap-2 list-none p-0 m-0">
        <li
          v-for="item in items"
          :key="item.id"
          :class="[
            'flex items-center justify-between p-3 bg-surface border border-border rounded-[10px]',
            item.isBought ? 'line-through opacity-50' : '',
          ]"
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

  const localItems = computed(() => (items.value !== null ? items.value : []));
</script>
