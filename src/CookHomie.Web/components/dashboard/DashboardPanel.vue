<template>
  <section class="bg-surface border border-border rounded-lg overflow-hidden">
    <header class="flex items-center justify-between px-5 py-4 border-b border-border">
      <h2 class="font-display text-base font-semibold text-text-primary">{{ title }}</h2>
      <button
        v-if="showRefresh"
        class="bg-transparent border border-border rounded-sm w-7 h-7 cursor-pointer text-text-secondary flex items-center justify-center transition-all hover:bg-surface-hover hover:text-text-primary disabled:opacity-40 disabled:cursor-not-allowed"
        :disabled="loading"
        @click="$emit('refresh')"
      >
        ↻
      </button>
    </header>
    <div v-if="loading && !hasData" class="p-5">
      <SharedSkeletonBlock class="h-[80px]" />
    </div>
    <SharedErrorBanner
      v-else-if="error && !hasData"
      :message="error"
      show-retry
      @retry="$emit('refresh')"
    />
    <SharedStaleIndicator v-else-if="isStale && hasData" @refresh="$emit('refresh')" />
    <div v-show="!loading || hasData" class="p-5">
      <slot />
    </div>
  </section>
</template>

<script setup lang="ts">
defineProps<{
  title: string;
  loading?: boolean;
  error?: string | null;
  isStale?: boolean;
  hasData?: boolean;
  showRefresh?: boolean;
}>();
defineEmits(["refresh"]);
</script>
