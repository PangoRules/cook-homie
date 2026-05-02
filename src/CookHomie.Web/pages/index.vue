<template>
  <div class="max-w-[960px] mx-auto p-6">
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
        @refresh="refresh"
      >
        <p v-if="!hasData && !loading" class="text-text-muted text-sm italic">
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
        <p v-if="!hasData && !loading" class="text-text-muted text-sm italic">
          Add inventory items to get recipe suggestions.
        </p>
      </DashboardPanel>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted } from "vue";

const { data, loading, error, isStale, start, stop, refresh } = useDashboard();

const hasData = computed(() => data.value !== null);

onMounted(async () => await start());
onUnmounted(() => stop());
</script>
