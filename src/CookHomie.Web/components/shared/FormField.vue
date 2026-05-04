<template>
  <div class="flex flex-col">
    <label class="font-semibold mb-1 text-sm text-text-primary">
      {{ label }}<span v-if="required" class="text-error"> *</span>
    </label>
    <slot />
    <span v-if="error" class="text-error text-xs mt-1">{{ error }}</span>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    label: string;
    error?: string;
    required?: boolean;
  }>();
</script>
