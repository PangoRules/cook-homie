<template>
  <span
    :class="[
      'inline-flex items-center gap-1 px-2 py-1 rounded-full text-[11px] font-semibold uppercase tracking-[0.05em]',
      isInStock
        ? 'bg-success-subtle text-success'
        : 'bg-error-subtle text-error'
    ]"
  >
    <span class="w-1.5 h-1.5 rounded-full bg-current" />
    {{ isInStock ? 'In stock' : 'Missing' }}
  </span>
</template>

<script setup lang="ts">
defineProps<{ isInStock: boolean }>();
</script>