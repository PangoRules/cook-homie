<template>
  <Teleport to="body">
    <div class="fixed bottom-6 right-6 z-[9999] flex flex-col gap-2 pointer-events-none" aria-live="polite">
      <TransitionGroup name="toast">
        <div
          v-for="toast in toasts"
          :key="toast.id"
          :class="[
            'flex items-center gap-3 px-4 py-3 rounded-md bg-surface shadow-lg border-l-4 cursor-pointer pointer-events-auto max-w-[360px] text-sm',
            toast.type === 'success' && 'border-l-success',
            toast.type === 'error'   && 'border-l-error',
            toast.type === 'warning' && 'border-l-warning',
            toast.type === 'info'    && 'border-l-info',
          ]"
          @click="remove(toast.id)"
        >
          <span class="text-base">{{ toastIcon(toast.type) }}</span>
          <span class="flex-1 text-text-primary">{{ toast.message }}</span>
          <button class="bg-transparent border-none cursor-pointer text-lg text-text-muted" aria-label="Dismiss">×</button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { useToast } from "@/composables/useToast";
const { toasts, remove } = useToast();

const toastIcon = (type: string) => {
  if (type === "success") return "✓";
  if (type === "error") return "✕";
  if (type === "warning") return "⚠";
  return "ℹ";
};
</script>
