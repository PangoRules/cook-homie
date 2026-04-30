<template>
  <div class="flex items-center gap-2 px-3 py-2 bg-warning-subtle rounded-sm text-xs text-warning">
    <span>⚡</span>
    <span>Data may be outdated</span>
    <button class="bg-transparent border-none underline cursor-pointer text-inherit text-xs" @click="$emit('refresh')">
      Refresh
    </button>
  </div>
</template>

<script setup lang="ts">
defineEmits(["refresh"]);
</script>
