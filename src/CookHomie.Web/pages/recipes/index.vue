<template>
  <div class="p-6">
    <header class="flex items-center justify-between mb-6">
      <h1 class="font-display text-[28px] font-bold">Recipes</h1>
      <SharedButton @click="showModal = true">+ Add Recipe</SharedButton>
    </header>

    <div class="mt-4">
      <SharedSkeletonBlock v-if="loading && localRecipes.length === 0" class="h-[120px]" />

      <SharedErrorBanner v-else-if="error" :message="error" @retry="refresh" />

      <p v-else-if="localRecipes.length === 0" class="text-text-muted text-sm">
        No recipes found. Add your first recipe!
      </p>

      <div v-else>
        <SharedStaleIndicator v-if="isStale" @refresh="refresh" />
        <div
          class="grid grid-cols-[repeat(auto-fill,minmax(300px,1fr))] gap-5 mt-5 max-sm:grid-cols-1"
        >
          <RecipesRecipeCard v-for="recipe in recipes" :key="recipe.id" :recipe="recipe" />
        </div>
      </div>
    </div>
    <RecipesAddRecipeModal v-if="showModal" @added="handleRecipeAdded" @close="onModalClose" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from "vue";
import { useRecipes } from "@/composables/useRecipes";
import { useToast } from "@/composables/useToast";

const { recipes, loading, error, isStale, start, stop, refresh } = useRecipes();
const { pushError } = useToast();
const showModal = ref(false);
const closing = ref(false);

const onModalClose = () => {
  closing.value = true;
  setTimeout(() => {
    showModal.value = false;
    closing.value = false;
  }, 200);
};

const handleRecipeAdded = () => {
  showModal.value = false;
  refresh().catch((err: unknown) => {
    pushError("Failed to refresh recipes after adding new one");
    console.error(err);
  });
};

onMounted(() => {
  start();
});

onUnmounted(() => stop());

const localRecipes = computed(() => (recipes.value !== null ? recipes.value : []));
</script>
