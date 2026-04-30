<template>
  <div class="fixed inset-0 bg-black/50 flex justify-center items-center z-[1000]" @click.self="$emit('close')">
    <div class="bg-surface rounded-lg shadow-lg w-[90%] max-w-lg max-h-[90vh] overflow-y-auto">
      <header class="flex justify-between items-center px-6 py-4 border-b border-border">
        <h2 class="font-display text-xl">Add Recipe</h2>
        <button class="bg-transparent border-none text-2xl cursor-pointer text-text-secondary hover:text-text-primary" @click="$emit('close')">×</button>
      </header>

      <form class="flex flex-col gap-4 p-6" @submit.prevent="handleSubmit">
        <div class="flex flex-col">
          <label class="font-semibold mb-1 text-sm text-text-primary">Name *</label>
          <input v-model="form.name" class="input" placeholder="e.g. Banana Bread" required />
          <span v-if="errors.name" class="text-error text-xs mt-1">{{ errors.name }}</span>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="flex flex-col">
            <label class="font-semibold mb-1 text-sm text-text-primary">Prep (min)</label>
            <input v-model.number="form.prepMinutes" type="number" min="0" class="input" />
          </div>
          <div class="flex flex-col">
            <label class="font-semibold mb-1 text-sm text-text-primary">Cook (min)</label>
            <input v-model.number="form.cookMinutes" type="number" min="0" class="input" />
          </div>
        </div>

        <div class="flex flex-col">
          <label class="font-semibold mb-1 text-sm text-text-primary">Instructions *</label>
          <textarea v-model="form.instructions" class="input" rows="4" placeholder="Step by step..." required />
          <span v-if="errors.instructions" class="text-error text-xs mt-1">{{ errors.instructions }}</span>
        </div>

        <div class="flex flex-col">
          <label class="font-semibold mb-1 text-sm text-text-primary">Tags (comma separated)</label>
          <input v-model="tagsInput" class="input" placeholder="quick, vegetarian" />
        </div>

        <div class="flex justify-end gap-2 pt-4 border-t border-border">
          <button type="button" class="btn btn-secondary" @click="$emit('close')">Cancel</button>
          <button type="submit" class="btn btn-primary" :disabled="submitting">
            {{ submitting ? "Adding…" : "Add Recipe" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { useToast } from "@/composables/useToast";

const emit = defineEmits(["added", "close"]);

const form = reactive({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  instructions: "",
  tags: [] as string[],
});

const tagsInput = ref("");
const errors = reactive({ name: "", instructions: "" });
const submitting = ref(false);

const validate = () => {
  let isValid = true;
  if (!form.name.trim()) { errors.name = "Recipe name is required"; isValid = false; }
  else errors.name = "";
  if (!form.instructions.trim()) { errors.instructions = "Instructions are required"; isValid = false; }
  else errors.instructions = "";
  return isValid;
};

const resetForm = () => {
  form.name = ""; form.prepMinutes = 0; form.cookMinutes = 0; form.instructions = "";
  tagsInput.value = ""; errors.name = ""; errors.instructions = "";
};

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    const toast = useToast();
    const response = await $fetch("/api/recipes", {
      method: "POST",
      body: {
        ...form,
        tags: tagsInput.value.split(",").map((t) => t.trim()).filter((t) => t.length > 0),
      },
    });
    emit("added", response);
    resetForm();
    toast.pushSuccess("Recipe added successfully!");
  } catch {
    useToast().pushError("Failed to add recipe");
  } finally {
    submitting.value = false;
  }
};
</script>
