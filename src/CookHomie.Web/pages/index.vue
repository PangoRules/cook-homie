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