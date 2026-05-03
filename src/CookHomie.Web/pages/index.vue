<template>
  <div class="p-6">
    <header class="flex items-center justify-between mb-6">
      <h1 class="font-display text-[28px] font-bold text-text-primary">Kitchen Overview</h1>
      <button
        class="bg-transparent border border-border rounded-md px-4 py-2 cursor-pointer text-sm text-text-secondary transition-all hover:bg-surface-hover hover:text-text-primary disabled:opacity-50 disabled:cursor-not-allowed"
        :disabled="loading"
        @click="refresh"
      >
        ↻ Refresh
      </button>
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
            <div v-for="item in data.expiringItems" :key="item.id" class="py-2 border-b border-border">
              <span class="text-text-primary font-medium">{{ item.name }}</span>
              <span class="block text-text-muted text-sm">{{ item.location }}</span>
              <span class="block text-warning text-xs font-semibold">{{ formatExpiry(item.expiresAt) }}</span>
            </div>
          </div>
          <div v-else-if="loading" class="text-text-muted text-sm italic">
            Loading expiring items...
          </div>
        </template>
      </DashboardPanel>

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
            <div v-for="idea in data.recipeIdeas" :key="idea.id" class="py-2 border-b border-border">
              <span class="text-text-primary font-medium">{{ idea.name }}</span>
              <span class="block text-text-muted text-sm">{{ idea.matchedCount }} matched, {{ idea.missingCount }} missing</span>
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
import { computed, onMounted, onUnmounted } from "vue";

const { data, loading, error, isStale, start, stop, refresh } = useDashboard();

const hasData = computed(() => data.value !== null);

function formatExpiry(expiryDate: string): string {
  const date = new Date(expiryDate);
  const today = new Date();
  const diffTime = date.getTime() - today.getTime();
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  
  if (diffDays === 0) return "Expires today";
  if (diffDays === 1) return "Expires tomorrow";
  if (diffDays < 0) return `Expired ${Math.abs(diffDays)} days ago`;
  
  return `Expires in ${diffDays} days`;
}

onMounted(async () => await start());
onUnmounted(() => stop());
</script>
