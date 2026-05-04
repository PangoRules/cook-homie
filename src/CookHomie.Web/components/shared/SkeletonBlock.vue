<template>
  <div
    class="skeleton"
    :class="$attrs.class"
    :style="{ borderRadius: radius }"
    aria-hidden="true"
  />
</template>

<script setup lang="ts">
  defineProps<{
    width?: string;
    height?: string;
    radius?: string;
  }>();
</script>
