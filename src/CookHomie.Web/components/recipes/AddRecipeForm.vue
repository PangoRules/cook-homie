<template>
  <div class="add-recipe-form">
    <h2>Add New Recipe</h2>
    <form @submit.prevent="submitForm">
      <div class="form-group">
        <label for="name">Recipe Name *</label>
        <input id="name" v-model="formData.name" type="text" required />
      </div>

      <div class="form-row">
        <div class="form-group">
          <label for="prepMinutes">Prep Time (minutes)</label>
          <input id="prepMinutes" v-model.number="formData.prepMinutes" type="number" min="0" />
        </div>

        <div class="form-group">
          <label for="cookMinutes">Cook Time (minutes)</label>
          <input id="cookMinutes" v-model.number="formData.cookMinutes" type="number" min="0" />
        </div>
      </div>

      <div class="form-group">
        <label for="tags">Tags (comma separated)</label>
        <input
          id="tags"
          v-model="tagsInput"
          type="text"
          placeholder="e.g. dinner, quick, healthy"
        />
      </div>

      <div class="form-group">
        <label for="instructions">Instructions *</label>
        <textarea id="instructions" v-model="formData.instructions" required rows="4" />
      </div>

      <button type="submit" :disabled="loading" class="submit-btn">
        {{ loading ? "Adding..." : "Add Recipe" }}
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import type { Recipe } from "@/types";

const formData = ref<Omit<Recipe, "id" | "createdAt" | "ingredients">>({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  tags: [],
  instructions: "",
});

const tagsInput = ref("");

// Watch tags input and update formData.tags when it changes
watch(tagsInput, (newTags) => {
  formData.value.tags = newTags
    .split(",")
    .map((tag) => tag.trim())
    .filter((tag) => tag);
});

const emit = defineEmits<{
  (e: "recipeAdded"): void;
}>();

const loading = ref(false);
const { pushSuccess, pushError } = useToast();

const submitForm = async () => {
  try {
    if (!formData.value.name || !formData.value.instructions) {
      pushError("Please fill in required fields");
      return;
    }

    loading.value = true;
    await $fetch<Recipe>("/api/recipes", {
      method: "POST",
      body: formData.value
    });
    emit("recipeAdded");
    pushSuccess("Recipe added successfully!");

    // Reset form
    formData.value = {
      name: "",
      prepMinutes: 0,
      cookMinutes: 0,
      tags: [],
      instructions: "",
    };
    tagsInput.value = "";
  } catch (err) {
    pushError("Failed to add recipe");
    console.error(err);
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.add-recipe-form {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;
}

.add-recipe-form h2 {
  margin-top: 0;
  color: #333;
}

.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  margin-bottom: 4px;
  font-weight: bold;
  color: #555;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1em;
  box-sizing: border-box;
}

.form-row {
  display: flex;
  gap: 16px;
}

.form-row .form-group {
  flex: 1;
}

.submit-btn {
  background-color: #2196f3;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1em;
  transition: background-color 0.2s;
}

.submit-btn:hover:not(:disabled) {
  background-color: #1976d2;
}

.submit-btn:disabled {
  background-color: #90caf9;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .form-row {
    flex-direction: column;
    gap: 0;
  }
}
</style>
