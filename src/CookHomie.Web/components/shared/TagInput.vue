<template>
  <div class="flex flex-col gap-2">
    <!-- Input row -->
    <div class="flex gap-2 flex-wrap">
      <input
        v-model="inputValue"
        type="text"
        class="input flex-1 min-w-[160px]"
        :placeholder="placeholder"
        @keydown.enter.prevent="addTag"
      >
      <SharedButton type="button" variant="secondary" @click="addTag"> + Add </SharedButton>
    </div>

    <!-- Tags display -->
    <div v-if="modelValue.length" class="flex gap-2 flex-wrap">
      <span v-for="(tag, index) in modelValue" :key="index" class="tag-chip">
        {{ tag }}
        <button
          type="button"
          class="leading-none text-current opacity-60 hover:opacity-100"
          @click="removeTag(index)"
        >
          ×
        </button>
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  modelValue: string[];
  placeholder?: string;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", tags: string[]): void;
}>();

const inputValue = ref("");

const addTag = () => {
  const raw = inputValue.value.trim();
  if (!raw) return;

  // Split on comma only — one tag per Enter press
  const newTags = raw
    .split(",")
    .map((t) => t.trim())
    .filter(Boolean);

  const merged = [...new Set([...props.modelValue, ...newTags])];
  emit("update:modelValue", merged);
  inputValue.value = "";
};

const removeTag = (index: number) => {
  const updated = props.modelValue.filter((_, i) => i !== index);
  emit("update:modelValue", updated);
};
</script>

