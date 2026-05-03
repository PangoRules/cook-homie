<template>
  <div class="flex items-center gap-3 px-4 py-3 bg-error-subtle border border-error rounded-md text-error text-sm" role="alert">
    <span>⚠</span>
    <span>{{ message }}</span>
    <button
      v-if="showRetry"
      class="ml-auto bg-transparent border border-current rounded-sm text-current px-3 py-1 cursor-pointer text-[13px]"
      @click="$emit('retry')"
    >
      Retry
    </button>
  </div>
</template>

<script setup lang="ts">
defineProps<{ message: string; showRetry?: boolean }>();
defineEmits(["retry"]);
</script>
