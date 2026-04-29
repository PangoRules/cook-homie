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