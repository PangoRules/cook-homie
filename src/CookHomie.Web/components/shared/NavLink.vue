<template>
  <NuxtLink
    :to="url"
    class="text-sm font-medium text-text-secondary hover:text-text-primary transition-colors"
    >{{ name }}</NuxtLink
  >
</template>

<script setup lang="ts">
  defineProps<{
    name: string;
    url: string;
  }>();
</script>
