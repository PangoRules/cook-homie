<template>
  <DevModeGuard>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </DevModeGuard>
</template>

<script setup lang="ts">
  import DevModeGuard from "@/components/shared/DevModeGuard.vue";
</script>
