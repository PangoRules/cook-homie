<template>
  <div class="recipes-page">
    <h1>Recipes</h1>

    <AddRecipeForm @recipe-added="handleRecipeAdded" />

    <div class="recipes-list">
      <SkeletonBlock v-if="loading && recipes.length === 0" :count="3" />

      <ErrorBanner v-else-if="error" :message="error" @retry="loadRecipes" />

      <div v-else-if="recipes.length === 0">
        <p>No recipes found. Add your first recipe using the form above.</p>
      </div>

      <div v-else>
        <StaleIndicator v-if="isStale" @refresh="loadRecipes" />

        <div class="recipes-grid">
          <RecipeCard v-for="recipe in recipes" :key="recipe.id" :recipe="recipe" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { useRecipes } from "@/composables/useRecipes";
import { useToast } from "@/composables/useToast";
import AddRecipeForm from "@/components/recipes/AddRecipeForm.vue";
import RecipeCard from "@/components/recipes/RecipeCard.vue";
import SkeletonBlock from "@/components/shared/SkeletonBlock.vue";
import ErrorBanner from "@/components/shared/ErrorBanner.vue";
import StaleIndicator from "@/components/shared/StaleIndicator.vue";

const { recipes, loading, error, isStale, loadRecipes, startPolling } = useRecipes();
const { pushError } = useToast();

const handleRecipeAdded = () => {
  // This will trigger a refresh which will include the new recipe
  loadRecipes().catch((err) => {
    pushError("Failed to refresh recipes after adding new one");
    console.error(err);
  });
};

onMounted(() => {
  startPolling();

  // Initial load
  loadRecipes().catch((err) => {
    pushError("Failed to load recipes");
    console.error(err);
  });
});
</script>

<style scoped>
.recipes-page {
  padding: 20px;
}

.recipes-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

@media (max-width: 768px) {
  .recipes-grid {
    grid-template-columns: 1fr;
  }

  .recipes-page {
    padding: 10px;
  }
}
</style>
