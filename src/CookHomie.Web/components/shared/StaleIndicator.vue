<!-- src/CookHomie.Web/components/shared/StaleIndicator.vue -->
<template>
  <div class="stale-indicator">
    <span>⚡</span>
    <span>Data may be outdated</span>
    <button @click="$emit('refresh')">Refresh</button>
  </div>
</template>

<script setup lang="ts">
defineEmits(["refresh"]);
</script>

<style scoped>
.stale-indicator {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-2) var(--space-3);
  background: var(--color-warning-subtle);
  border-radius: var(--radius-sm);
  font-size: 12px;
  color: var(--color-warning);
}

.stale-indicator button {
  background: none;
  border: none;
  text-decoration: underline;
  cursor: pointer;
  color: inherit;
  font-size: inherit;
}
</style>
