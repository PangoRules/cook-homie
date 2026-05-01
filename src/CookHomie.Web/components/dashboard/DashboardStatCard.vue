<template>
  <div
    :class="[
      'bg-surface border border-border rounded-lg p-5 shadow-sm cursor-pointer transition-all flex flex-col gap-1 hover:shadow-md hover:-translate-y-px',
      variant === 'warning' && 'border-l-4 border-l-warning',
      variant === 'success' && 'border-l-4 border-l-success',
    ]"
    @click="$emit('click')"
  >
    <div class="text-xs font-semibold uppercase tracking-[0.06em] text-text-secondary">
      {{ label }}
    </div>
    <div v-if="loading" class="p-5">
      <SharedSkeletonBlock class="h-5" />
    </div>
    <div
      v-else
      class="text-[26px] sm:text-[32px] font-bold text-text-primary font-display leading-[1.1]"
    >
      {{ displayValue }}
    </div>
    <div v-if="sub" class="text-xs text-text-muted mt-1">{{ sub }}</div>
    <slot />
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

const props = defineProps<{
  label: string;
  value: number | string;
  sub?: string;
  variant?: "default" | "warning" | "success";
  loading: boolean;
}>();

defineEmits(["click"]);

const displayValue = computed(() =>
  typeof props.value === "number" ? props.value.toLocaleString() : props.value
);
</script>
