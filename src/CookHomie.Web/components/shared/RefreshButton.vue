<template>
  <button
    class="bg-transparent border border-border rounded-md px-4 py-2 cursor-pointer text-sm text-text-secondary transition-all hover:bg-surface-hover hover:text-text-primary disabled:opacity-50 disabled:cursor-not-allowed"
    :disabled="loading"
    @click="$emit('refresh')"
  >
    ↻ {{ label }}
  </button>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  loading?: boolean;
  label?: string;
}>(), {
  loading: false,
  label: "Refresh",
});
defineEmits(["refresh"]);
</script>