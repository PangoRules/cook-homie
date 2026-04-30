<template>
  <div class="p-6">
    <h1>Recipes</h1>

    <RecipesAddRecipeModal @added="handleRecipeAdded" />

    <div class="mt-4">
      <SharedSkeletonBlock v-if="loading && recipes.length === 0" :count="3" />

      <SharedErrorBanner v-else-if="error" :message="error" @retry="refresh" />

      <div v-else-if="recipes.length === 0">
        <p>No recipes found. Add your first recipe using the form above.</p>
      </div>

      <div v-else>
        <SharedStaleIndicator v-if="isStale" @refresh="refresh" />

        <div class="grid grid-cols-[repeat(auto-fill,minmax(300px,1fr))] gap-5 mt-5 max-sm:grid-cols-1">
          <RecipesRecipeCard v-for="recipe in recipes" :key="recipe.id" :recipe="recipe" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from "vue";
import { useRecipes } from "@/composables/useRecipes";
import { useToast } from "@/composables/useToast";

const { recipes, loading, error, isStale, start, stop, refresh } = useRecipes();
const { pushError } = useToast();

const handleRecipeAdded = () => {
  refresh().catch((err: unknown) => {
    pushError("Failed to refresh recipes after adding new one");
    console.error(err);
  });
};

onMounted(() => {
  start();
  refresh().catch((err: unknown) => {
    pushError("Failed to load recipes");
    console.error(err);
  });
});

onUnmounted(() => stop());
</script>
