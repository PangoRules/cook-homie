<template>
  <div class="recipe-card">
    <div class="recipe-header">
      <h3>{{ recipe.name }}</h3>
      <div class="recipe-meta">
        <span class="prep-time">Prep: {{ recipe.prepMinutes }}m</span>
        <span class="cook-time">Cook: {{ recipe.cookMinutes }}m</span>
      </div>
    </div>

    <div class="recipe-tags">
      <span v-for="tag in recipe.tags" :key="tag" class="tag">
        {{ tag }}
      </span>
    </div>

    <div class="recipe-description">
      <p>
        {{ recipe.instructions.substring(0, 150)
        }}{{ recipe.instructions.length > 150 ? "..." : "" }}
      </p>
    </div>

    <div class="recipe-actions">
      <button class="add-to-shopping-btn" @click="addToShoppingList">Add to Shopping List</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Recipe } from "@/types";

const props = defineProps<{
  recipe: Recipe;
}>();

const { addToList } = useShoppingList();
const { pushSuccess, pushError } = useToast();

const addToShoppingList = async () => {
  try {
    // For simplicity, we'll add all ingredients to shopping list
    // In a real implementation, you would want to select ingredients
    await addToList(props.recipe.ingredients.map((i) => i.ingredientName));
    pushSuccess("Recipe ingredients added to shopping list!");
  } catch (err) {
    pushError("Failed to add recipe ingredients to shopping list");
    console.error(err);
  }
};
</script>

<style scoped>
.recipe-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 16px;
  background: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.recipe-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 12px;
}

.recipe-header h3 {
  margin: 0;
  font-size: 1.2em;
}

.recipe-meta {
  display: flex;
  gap: 12px;
  font-size: 0.9em;
  color: #666;
}

.recipe-tags {
  margin: 12px 0;
}

.tag {
  background-color: #e0f7fa;
  color: #00838f;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.8em;
  margin-right: 8px;
}

.recipe-description {
  margin: 12px 0;
  color: #333;
  line-height: 1.4;
}

.recipe-actions {
  display: flex;
  justify-content: flex-end;
}

.add-to-shopping-btn {
  background-color: #4caf50;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9em;
}

.add-to-shopping-btn:hover {
  background-color: #388e3c;
}
</style>
