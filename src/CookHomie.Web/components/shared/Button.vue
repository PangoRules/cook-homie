<template>
  <button
    v-bind="$attrs"
    :type="type"
    :disabled="disabled"
    :class="['btn', variant === 'primary' ? 'btn-primary' : 'btn-secondary']"
  >
    <slot />
  </button>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  variant?: 'primary' | 'secondary';
  type?: 'button' | 'submit' | 'reset';
  disabled?: boolean;
}>(), {
  variant: 'primary',
  type: 'button',
});
</script>
