<template>
  <section class="bg-surface border border-border rounded-lg overflow-hidden">
    <header class="flex items-center justify-between px-5 py-4 border-b border-border">
      <h2 class="font-display text-base font-semibold text-text-primary">{{ title }}</h2>
      <RefreshButton v-if="showRefresh" :loading="loading" @click="$emit('refresh')" />
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
  import RefreshButton from "~/components/shared/RefreshButton.vue";

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
