<template>
  <div class="bg-surface p-5 rounded-lg shadow-sm mb-5">
    <h2 class="text-text-primary mt-0 mb-4">Add New Recipe</h2>
    <form class="flex flex-col gap-4" @submit.prevent="submitForm">
      <SharedFormField label="Recipe Name" required>
        <input id="name" v-model="formData.name" type="text" class="input" required />
      </SharedFormField>

      <div class="flex gap-4 max-sm:flex-col">
        <SharedFormField label="Prep Time (minutes)" class="flex-1">
          <input id="prepMinutes" v-model.number="formData.prepMinutes" type="number" min="0" class="input" />
        </SharedFormField>
        <SharedFormField label="Cook Time (minutes)" class="flex-1">
          <input id="cookMinutes" v-model.number="formData.cookMinutes" type="number" min="0" class="input" />
        </SharedFormField>
      </div>

      <SharedFormField label="Tags (comma separated)">
        <input id="tags" v-model="tagsInput" type="text" class="input" placeholder="e.g. dinner, quick, healthy" />
      </SharedFormField>

      <SharedFormField label="Instructions" required>
        <textarea id="instructions" v-model="formData.instructions" class="input" required rows="4" />
      </SharedFormField>

      <SharedButton type="submit" :disabled="loading">
        {{ loading ? "Adding..." : "Add Recipe" }}
      </SharedButton>
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
