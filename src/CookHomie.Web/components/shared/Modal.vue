<template>
  <Teleport to="body">
    <div
      class="fixed inset-0 bg-black/50 flex justify-center items-center z-[1000]"
      :class="closing ? 'animate-backdrop-out' : 'animate-backdrop-in'"
      @click.self="startClose"
    >
      <div
        class="bg-surface rounded-lg shadow-lg w-[90%] overflow-y-auto"
        :class="[maxWidth, closing ? 'animate-modal-out' : 'animate-modal-in']"
        :style="maxHeight ? { maxHeight } : { maxHeight: '90vh' }"
      >
        <header class="flex justify-between items-center px-6 py-4 border-b border-border">
          <slot name="header">
            <h2 class="font-display text-xl">{{ title }}</h2>
          </slot>
          <button
            class="bg-transparent border-none text-2xl cursor-pointer text-text-secondary hover:text-text-primary leading-none"
            aria-label="Close"
            @click="startClose"
          >
            ×
          </button>
        </header>

        <div class="p-6">
          <slot />
        </div>

        <footer
          v-if="$slots.footer"
          class="flex justify-end gap-2 px-6 py-4 border-t border-border"
        >
          <slot name="footer" />
        </footer>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
  import { ref } from "vue";

  withDefaults(
    defineProps<{
      title?: string;
      maxWidth?: string;
      maxHeight?: string;
    }>(),
    {
      title: "",
      maxWidth: "max-w-lg",
      maxHeight: "",
    }
  );

  const emit = defineEmits<{ (e: "close"): void }>();
  const closing = ref(false);

  const startClose = () => {
    closing.value = true;
    setTimeout(() => emit("close"), 180);
  };

  defineExpose({ startClose });
</script>
