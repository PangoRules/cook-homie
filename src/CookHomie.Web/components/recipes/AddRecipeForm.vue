<template>
  <div class="bg-surface p-5 rounded-lg shadow-sm mb-5">
    <h2 class="text-text-primary mt-0 mb-4">Add New Recipe</h2>
    <form @submit.prevent="submitForm">
      <div class="mb-4">
        <label class="block mb-1 text-sm font-semibold text-text-secondary" for="name">Recipe Name *</label>
        <input id="name" v-model="formData.name" type="text" class="input" required />
      </div>

      <div class="flex gap-4 mb-4 max-sm:flex-col max-sm:gap-0">
        <div class="flex-1 mb-4">
          <label class="block mb-1 text-sm font-semibold text-text-secondary" for="prepMinutes">Prep Time (minutes)</label>
          <input id="prepMinutes" v-model.number="formData.prepMinutes" type="number" min="0" class="input" />
        </div>
        <div class="flex-1 mb-4">
          <label class="block mb-1 text-sm font-semibold text-text-secondary" for="cookMinutes">Cook Time (minutes)</label>
          <input id="cookMinutes" v-model.number="formData.cookMinutes" type="number" min="0" class="input" />
        </div>
      </div>

      <div class="mb-4">
        <label class="block mb-1 text-sm font-semibold text-text-secondary" for="tags">Tags (comma separated)</label>
        <input id="tags" v-model="tagsInput" type="text" class="input" placeholder="e.g. dinner, quick, healthy" />
      </div>

      <div class="mb-4">
        <label class="block mb-1 text-sm font-semibold text-text-secondary" for="instructions">Instructions *</label>
        <textarea id="instructions" v-model="formData.instructions" class="input" required rows="4" />
      </div>

      <button type="submit" :disabled="loading" class="btn btn-primary">
        {{ loading ? "Adding..." : "Add Recipe" }}
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import type { Recipe } from "@/types";

const formData = ref<Omit<Recipe, "id" | "createdAt" | "ingredients">>({
  name: "", prepMinutes: 0, cookMinutes: 0, tags: [], instructions: "",
});

const tagsInput = ref("");

watch(tagsInput, (newTags) => {
  formData.value.tags = newTags.split(",").map((tag) => tag.trim()).filter((tag) => tag);
});

const emit = defineEmits<{ (e: "recipeAdded"): void }>();
const loading = ref(false);
const { pushSuccess, pushError } = useToast();

const submitForm = async () => {
  try {
    if (!formData.value.name || !formData.value.instructions) {
      pushError("Please fill in required fields");
      return;
    }
    loading.value = true;
    await $fetch<Recipe>("/api/recipes", { method: "POST", body: formData.value });
    emit("recipeAdded");
    pushSuccess("Recipe added successfully!");
    formData.value = { name: "", prepMinutes: 0, cookMinutes: 0, tags: [], instructions: "" };
    tagsInput.value = "";
  } catch (err) {
    pushError("Failed to add recipe");
    console.error(err);
  } finally {
    loading.value = false;
  }
};
</script>
