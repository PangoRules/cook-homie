<template>
  <DashboardPanel
    title="Shopping List"
    :loading="loading"
    :error="error"
    :is-stale="isStale"
    :has-data="items.length > 0"
    :show-refresh="true"
    @refresh="refreshShoppingList"
  >
    <!-- Loading skeleton -->
    <div v-if="loading && items.length === 0" class="space-y-3">
      <SharedSkeletonBlock width="100%" height="40px" />
      <SharedSkeletonBlock width="100%" height="40px" />
      <SharedSkeletonBlock width="100%" height="40px" />
    </div>

    <!-- Error state -->
    <SharedErrorBanner
      v-else-if="error && items.length === 0"
      :message="error"
      @retry="refreshShoppingList"
    />

    <!-- Empty state -->
    <div v-else-if="items.length === 0" class="text-center py-12">
      <p class="text-text-secondary mb-4">Your shopping list is empty</p>
      <SharedButton variant="primary">Add Items</SharedButton>
    </div>

    <!-- Shopping items -->
    <div v-else class="space-y-3">
      <div
        v-for="item in items"
        :key="item.id"
        class="flex items-center justify-between p-4 bg-surface border border-border rounded-lg"
      >
        <div>
          <h3 class="font-medium">{{ item.name }}</h3>
          <p class="text-sm text-text-secondary">{{ item.quantity }} {{ item.unit }}</p>
        </div>
        <div class="flex gap-2">
          <SharedButton variant="secondary" size="sm">Edit</SharedButton>
          <SharedButton variant="secondary" size="sm">Remove</SharedButton>
        </div>
      </div>
    </div>
  </DashboardPanel>
</template>

<script setup lang="ts">
import { useShoppingList } from "@/composables/useShoppingList";

const { items, loading, error, isStale, loadList, refresh } = useShoppingList();

// Refresh the shopping list
const refreshShoppingList = async () => {
  try {
    await refresh();
  } catch {
    // Error state is already handled by composable
  }
};

// Load shopping list on component mount
try {
  await loadList();
} catch {
  // Error state is already handled by composable
}

// Start polling for updates
onMounted(() => {
  const { startPolling } = useShoppingList();
  startPolling();
});

onUnmounted(() => {
  const { stopPolling } = useShoppingList();
  stopPolling();
});
</script>
