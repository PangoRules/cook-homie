<template>
  <div class="p-6">
    <header class="flex items-center justify-between mb-6">
      <h1 class="font-display text-[28px] font-bold">Inventory</h1>
      <SharedButton @click="showModal = true">+ Add Item</SharedButton>
    </header>

    <DashboardPanel
      title=""
      :loading="loading"
      :error="error"
      :is-stale="isStale"
      :has-data="itemsLocalCopy.length > 0"
      show-refresh
      @refresh="refresh"
    >
      <div v-if="loading && itemsLocalCopy.length === 0" class="flex flex-col gap-2">
        <SharedSkeletonBlock v-for="i in 5" :key="i" class="h-12" />
      </div>
      <SharedErrorBanner
        v-else-if="error && itemsLocalCopy.length === 0"
        :message="error"
        show-retry
        @retry="refresh"
      />
      <p v-else-if="itemsLocalCopy.length === 0" class="text-text-muted text-sm text-center py-10">
        No inventory items. Add your first item above.
      </p>
      <ul v-else class="flex flex-col gap-2 list-none p-0 m-0">
        <li
          v-for="item in items"
          :key="item.id"
          class="flex items-center justify-between p-3 bg-surface border border-border rounded-[10px] hover:shadow-sm transition-shadow cursor-pointer"
          @click="openItem(item)"
        >
          <div class="flex flex-col gap-0.5">
            <span class="font-semibold text-[15px]">{{ item.name }}</span>
            <span class="text-xs text-text-muted"
              >{{ item.quantity }} {{ item.unit }} · {{ item.location }}</span
            >
          </div>
          <span
            :class="['text-xs', item.expiresAt ? 'text-warning font-semibold' : 'text-text-muted']"
          >
            {{ item.expiresAt ? formatExpiryShort(item.expiresAt) : "No expiry" }}
          </span>
        </li>
      </ul>
    </DashboardPanel>

    <InventoryAddItemModal v-if="showModal" @added="handleAdded" @close="showModal = false" />
    <InventoryItemDetailModal
      v-if="selectedItem"
      :item="selectedItem"
      mode="inventory"
      @close="showDetailModal = false"
      @edited="handleEdited"
    />
  </div>
</template>

<script setup lang="ts">
  import { computed, onMounted, onUnmounted, ref } from "vue";
  import { formatExpiryShort } from "~/utils/date";
  import InventoryItemDetailModal from "~/components/inventory/InventoryItemDetailModal.vue";
  import type { InventoryItem } from "~/types";

  const { items, loading, error, isStale, startPolling, stopPolling, refresh } = useInventory();
  const showModal = ref(false);
  const selectedItem = ref<InventoryItem | null>(null);
  const showDetailModal = ref(false);

  onMounted(() => startPolling());
  onUnmounted(() => stopPolling());

  const handleAdded = () => {
    showModal.value = false;
    refresh();
  };

  const openItem = (item: InventoryItem) => {
    selectedItem.value = item;
    showDetailModal.value = true;
  };

  const handleEdited = () => {
    showDetailModal.value = false;
    refresh();
  };

  const itemsLocalCopy = computed(() => (items.value !== null ? items.value : []));
</script>
