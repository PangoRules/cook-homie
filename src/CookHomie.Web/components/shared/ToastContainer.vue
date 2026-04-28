<!-- src/CookHomie.Web/components/shared/ToastContainer.vue -->
<template>
  <Teleport to="body">
    <div class="toast-container" aria-live="polite">
      <TransitionGroup name="toast">
        <div
          v-for="toast in toasts"
          :key="toast.id"
          :class="['toast', `toast--${toast.type}`]"
          @click="remove(toast.id)"
        >
          <span class="toast__icon">{{ toastIcon(toast.type) }}</span>
          <span class="toast__message">{{ toast.message }}</span>
          <button class="toast__close" aria-label="Dismiss">×</button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { useToast } from "~/composables/useToast";
const { toasts, remove } = useToast();

const toastIcon = (type: string) => {
  if (type === "success") return "✓";
  if (type === "error") return "✕";
  if (type === "warning") return "⚠";
  return "ℹ";
};
</script>

<style scoped>
.toast-container {
  position: fixed;
  bottom: var(--space-6);
  right: var(--space-6);
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
  pointer-events: none;
}

.toast {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  background: var(--color-surface);
  box-shadow: var(--shadow-lg);
  border-left: 4px solid;
  cursor: pointer;
  pointer-events: all;
  max-width: 360px;
  font-family: var(--font-body);
  font-size: 14px;
}

.toast--success { border-color: var(--color-success); }
.toast--error { border-color: var(--color-error); }
.toast--warning { border-color: var(--color-warning); }
.toast--info { border-color: var(--color-info); }

.toast__icon { font-size: 16px; }
.toast__message { flex: 1; color: var(--color-text-primary); }
.toast__close { background: none; border: none; cursor: pointer; font-size: 18px; color: var(--color-text-muted); }

.toast-enter-active, .toast-leave-active { transition: all var(--transition-base); }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateX(20px); }
</style>