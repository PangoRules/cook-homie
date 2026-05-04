<!-- src/CookHomie.Web/components/shared/DevModeGuard.vue -->
<template>
  <slot v-if="isDev" />
</template>

<script setup lang="ts">
  import { useDevMode } from "@/composables/useDevMode";

  const { isDev } = useDevMode();
</script>
