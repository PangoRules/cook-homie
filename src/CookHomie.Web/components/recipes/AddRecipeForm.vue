<template>
  <div class="bg-surface p-5 rounded-lg shadow-sm mb-5">
    <h2 class="text-text-primary mt-0 mb-4">Add New Recipe</h2>
    <form class="flex flex-col gap-4" @submit.prevent="submitForm">
      <SharedFormField label="Recipe Name" required>
        <input id="name" v-model="formData.name" type="text" class="input" required >
      </SharedFormField>

      <div class="flex gap-4 max-sm:flex-col">
        <SharedFormField label="Prep Time (minutes)" class="flex-1">
          <input
            id="prepMinutes"
            v-model.number="formData.prepMinutes"
            type="number"
            min="0"
            class="input"
          >
        </SharedFormField>
        <SharedFormField label="Cook Time (minutes)" class="flex-1">
          <input
            id="cookMinutes"
            v-model.number="formData.cookMinutes"
            type="number"
            min="0"
            class="input"
          >
        </SharedFormField>
      </div>

      <SharedFormField label="Tags">
        <SharedTagInput v-model="formData.tags" placeholder="Type tag, press Enter" />
      </SharedFormField>

      <SharedFormField label="Instructions" required>
        <textarea
          id="instructions"
          v-model="formData.instructions"
          class="input"
          required
          rows="4"
        />
      </SharedFormField>

      <SharedButton type="submit" :disabled="loading">
        {{ loading ? "Adding..." : "Add Recipe" }}
      </SharedButton>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import type { Recipe, AddRecipePayload } from "@/types";

const formData = ref<AddRecipePayload>({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  tags: [],
  instructions: "",
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
  } catch (err) {
    pushError("Failed to add recipe");
    console.error(err);
  } finally {
    loading.value = false;
  }
};
</script>
