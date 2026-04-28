<!-- src/CookHomie.Web/components/shared/SkeletonBlock.vue -->
<template>
  <div
    class="skeleton"
    :style="{ width: width, height: height, borderRadius: radius }"
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

<style scoped>
.skeleton {
  background: linear-gradient(
    90deg,
    var(--color-border) 25%,
    var(--color-surface-hover) 50%,
    var(--color-border) 75%
  );
  background-size: 200% 100%;
  animation: shimmer 1.4s ease infinite;
  border-radius: var(--radius-md);
}

@keyframes shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}
</style>
