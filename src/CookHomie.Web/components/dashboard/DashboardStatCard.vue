<template>
  <div :class="['stat-card', variant]" @click="$emit('click')">
    <div class="stat-card__label">{{ label }}</div>
    <div class="stat-card__value">{{ displayValue }}</div>
    <div v-if="sub" class="stat-card__sub">{{ sub }}</div>
    <slot />
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  label: string;
  value: number | string;
  sub?: string;
  variant?: "default" | "warning" | "success";
}>();

defineEmits(["click"]);

const displayValue = computed(() =>
  typeof props.value === "number" ? props.value.toLocaleString() : props.value
);
</script>

<style scoped>
.stat-card {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: var(--space-5);
  box-shadow: var(--shadow-sm);
  cursor: pointer;
  transition: box-shadow var(--transition-fast), transform var(--transition-fast);
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

.stat-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-1px);
}

.stat-card__label {
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: var(--color-text-secondary);
  font-family: var(--font-body);
}

.stat-card__value {
  font-size: 32px;
  font-weight: 700;
  color: var(--color-text-primary);
  font-family: var(--font-display);
  line-height: 1.1;
}

.stat-card__sub {
  font-size: 12px;
  color: var(--color-text-muted);
  margin-top: var(--space-1);
}

.stat-card.warning { border-left: 4px solid var(--color-warning); }
.stat-card.success { border-left: 4px solid var(--color-success); }

@media (max-width: 480px) {
  .stat-card__value { font-size: 26px; }
}
</style>