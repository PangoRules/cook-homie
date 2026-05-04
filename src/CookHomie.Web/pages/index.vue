<template>
  <div class="p-6">
    <header class="flex items-center justify-between mb-6">
      <h1 class="font-display text-[28px] font-bold text-text-primary">Kitchen Overview</h1>
      <RefreshButton :loading="loading" @refresh="refresh" />
    </header>

    <div class="grid grid-cols-3 gap-4 mb-6 max-md:grid-cols-1">
      <DashboardStatCard
        label="Expiring Soon"
        :loading="loading"
        :value="data?.expiringCount ?? 0"
        sub="items within 3 days"
        :variant="(data?.expiringCount ?? 0) > 0 ? 'warning' : 'success'"
        @click="navigateTo('/inventory')"
      />
      <DashboardStatCard
        label="Recipe Matches"
        :loading="loading"
        :value="data?.recipeMatchCount ?? 0"
        sub="cookable from inventory"
        @click="navigateTo('/recipes')"
      />
      <DashboardStatCard
        label="Shopping List"
        :loading="loading"
        :value="data?.shoppingCount ?? 0"
        sub="items pending"
        @click="navigateTo('/shopping')"
      />
    </div>

    <div class="grid grid-cols-2 gap-4 max-md:grid-cols-1">
      <DashboardPanel
        title="Expiring Soon"
        :loading="loading"
        :error="error"
        :is-stale="isStale"
        :has-data="hasData"
        show-refresh
        data-testid="expiring-items-panel"
        @refresh="refresh"
      >
        <template #default>
          <div v-if="!hasData && !loading" class="text-text-muted text-sm italic">
            No expiring items — inventory looks fresh!
          </div>
          <div v-else-if="data?.expiringItems && data.expiringItems.length > 0">
            <div
              v-for="item in data.expiringItems"
              :key="item.id"
              class="py-2 border-b border-border cursor-pointer"
              @click="openExpiring(item)"
            >
              <span class="text-text-primary font-medium">{{ item.name }}</span>
              <span class="block text-text-muted text-sm">{{ item.location }}</span>
              <span class="block text-warning text-xs font-semibold">{{
                formatExpiryLong(item.expiresAt)
              }}</span>
            </div>
          </div>
          <div v-else-if="loading" class="text-text-muted text-sm italic">
            Loading expiring items...
          </div>
        </template>
      </DashboardPanel>

      <InventoryItemDetailModal
        v-if="showExpiringModal"
        :item="expiringItemDetails"
        mode="expiring"
        @close="showExpiringModal = false"
        @dismiss="handleExpiringDismiss"
        @discard="handleExpiringDismiss"
        @restocked="handleExpiringRestocked"
        @add-to-shopping-list="handleExpiringAddToShoppingList"
      />

      <DashboardPanel
        title="Quick Recipe Ideas"
        :loading="loading"
        :error="error"
        :is-stale="isStale"
        :has-data="hasData"
        show-refresh
        data-testid="recipe-ideas-panel"
        @refresh="refresh"
      >
        <template #default>
          <div v-if="!hasData && !loading" class="text-text-muted text-sm italic">
            Add inventory items to get recipe suggestions.
          </div>
          <div v-else-if="data?.recipeIdeas && data.recipeIdeas.length > 0">
            <div v-for="idea in data.recipeIdeas" :key="idea.id" class="border-b border-border">
              <div
                class="flex items-center justify-between py-2 cursor-pointer"
                @click="toggleExpand(idea)"
              >
                <div>
                  <NuxtLink
                    :to="`/recipes/${idea.id}`"
                    class="text-text-primary font-medium hover:underline focus:outline-none focus:ring-2 focus:ring-accent rounded"
                  >
                    {{ idea.name }}
                  </NuxtLink>
                  <span class="block text-text-muted text-sm"
                    >{{ idea.matchedCount }} matched, {{ idea.missingCount }} missing</span
                  >
                </div>
                <span v-if="idea.missingCount > 0">{{
                  expandedIdeaId === idea.id ? "▲" : "▼"
                }}</span>
              </div>

              <!-- Expand UI stubs awaiting Task 9 backend (GET /api/recipes/{id}/missing not implemented) -->
              <div v-if="expandedIdeaId === idea.id" class="pl-4 pb-3">
                <div v-if="loadingMissing" class="text-text-muted text-sm">Loading…</div>
                <div v-else-if="missingCache[idea.id]?.length > 0">
                  <ul class="list-none p-0 m-0 flex flex-col gap-1">
                    <li
                      v-for="ing in missingCache[idea.id]"
                      :key="ing"
                      class="flex items-center justify-between text-sm"
                    >
                      <span>{{ ing }}</span>
                      <SharedButton
                        size="small"
                        variant="secondary"
                        @click.stop="addMissing(idea.id, ing)"
                      >
                        + Add
                      </SharedButton>
                    </li>
                  </ul>
                  <SharedButton
                    class="mt-2"
                    variant="primary"
                    size="small"
                    @click.stop="addAllMissing(idea.id)"
                  >
                    Add all missing
                  </SharedButton>
                </div>
                <div v-else class="text-text-muted text-sm">
                  Missing ingredients unavailable (API stub)
                </div>
              </div>
            </div>
          </div>
          <div v-else-if="loading" class="text-text-muted text-sm italic">
            Loading recipe suggestions...
          </div>
        </template>
      </DashboardPanel>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { computed, onMounted, onUnmounted, ref } from "vue";
  import { formatExpiryLong } from "~/utils/date";
  import RefreshButton from "~/components/shared/RefreshButton.vue";
  import type { DashboardExpiringItem, InventoryItem } from "~/types";
  import InventoryItemDetailModal from "~/components/inventory/InventoryItemDetailModal.vue";

  const { data, loading, error, isStale, start, stop, refresh } = useDashboard();

  const hasData = computed(() => data.value !== null);

  const selectedExpiringItem = ref<DashboardExpiringItem | null>(null);
  const showExpiringModal = ref(false);
  const expiringItemDetails = ref<InventoryItem | null>(null);

  const openExpiring = async (item: DashboardExpiringItem) => {
    // RLOG
    console.log("🚀 index.vue:174 ╎item╎:", item); // RLOG_END
    selectedExpiringItem.value = item;
    showExpiringModal.value = true;
    const { items, refresh } = useInventory();
    await refresh();
    console.log("items", items.value);
    expiringItemDetails.value = items.value?.find((i) => i.id === item.id) ?? null;
  };

  const handleExpiringDismiss = () => {
    showExpiringModal.value = false;
    refresh();
  };

  const handleExpiringRestocked = () => {
    showExpiringModal.value = false;
    refresh();
  };

  const handleExpiringAddToShoppingList = async () => {
    if (!selectedExpiringItem.value) return;
    const { addBulk } = useShoppingList();
    try {
      await addBulk([selectedExpiringItem.value.name]);
      useToast().pushSuccess(`"${selectedExpiringItem.value.name}" added to shopping list`);
    } catch {
      useToast().pushError("Failed to add to shopping list");
    }
    showExpiringModal.value = false;
    refresh();
  };

  const expandedIdeaId = ref<string | null>(null);
  const loadingMissing = ref(false);
  const missingCache = ref<Record<string, string[]>>({});

  const toggleExpand = async (idea: { id: string }) => {
    if (expandedIdeaId.value === idea.id) {
      expandedIdeaId.value = null;
      return;
    }
    expandedIdeaId.value = idea.id;
    if (!missingCache.value[idea.id]) {
      loadingMissing.value = true;
      try {
        const missing = await $fetch<string[]>(`/api/recipes/${idea.id}/missing`);
        missingCache.value[idea.id] = missing;
      } finally {
        loadingMissing.value = false;
      }
    }
  };

  const addMissing = async (recipeId: string, ingredientName: string) => {
    const { addBulk } = useShoppingList();
    try {
      await addBulk([ingredientName]);
      useToast().pushSuccess(`"${ingredientName}" added to shopping list`);
    } catch {
      useToast().pushError("Already on shopping list");
    }
  };

  const addAllMissing = async (recipeId: string) => {
    const { addBulk } = useShoppingList();
    const missing = missingCache.value[recipeId] ?? [];
    try {
      await addBulk(missing);
      useToast().pushSuccess(`${missing.length} items added to shopping list`);
    } catch (err) {
      const isPartial =
        err &&
        typeof err === "object" &&
        (err as { isPartialFailure?: boolean }).isPartialFailure === true;
      useToast().pushError(isPartial ? "Some items already on list" : "Failed to add items");
    }
  };

  onMounted(async () => await start());
  onUnmounted(() => stop());
</script>
