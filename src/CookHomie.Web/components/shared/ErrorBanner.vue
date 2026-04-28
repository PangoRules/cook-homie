<!-- src/CookHomie.Web/components/shared/ErrorBanner.vue -->
<template>
  <div class="error-banner" role="alert">
    <span class="error-banner__icon">⚠</span>
    <span class="error-banner__message">{{ message }}</span>
    <button v-if="showRetry" class="error-banner__retry" @click="$emit('retry')">Retry</button>
  </div>
</template>

<script setup lang="ts">
defineProps<{ message: string; showRetry?: boolean }>();
defineEmits(["retry"]);
</script>

<style scoped>
.error-banner {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-3) var(--space-4);
  background: var(--color-error-subtle);
  border: 1px solid var(--color-error);
  border-radius: var(--radius-md);
  color: var(--color-error);
  font-family: var(--font-body);
  font-size: 14px;
}

.error-banner__retry {
  margin-left: auto;
  background: none;
  border: 1px solid currentColor;
  border-radius: var(--radius-sm);
  color: inherit;
  padding: var(--space-1) var(--space-3);
  cursor: pointer;
  font-size: 13px;
}
</style>