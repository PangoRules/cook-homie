<template>
  <div class="max-w-[960px] mx-auto p-6">
    <header class="flex items-center justify-between mb-6 max-sm:flex-col max-sm:items-start max-sm:gap-3">
      <h1 class="font-display text-[28px] font-bold">Recipes</h1>
      <SharedButton @click="showModal = true">+ Add Recipe</SharedButton>
    </header>

    <DashboardPanel
      title=""
      :loading="loading"
      :error="error"
      :is-stale="isStale"
      :has-data="recipes.length > 0"
      show-refresh
      @refresh="refresh"
    >
      <div v-if="loading && recipes.length === 0" class="grid grid-cols-[repeat(auto-fill,minmax(260px,1fr))] gap-4">
        <SharedSkeletonBlock v-for="i in 3" :key="i" class="h-[120px]" />
      </div>
      <div v-else-if="error && recipes.length === 0">
        <SharedErrorBanner :message="error" show-retry @retry="refresh" />
      </div>
      <p v-else-if="recipes.length === 0" class="text-center py-10 text-text-muted">
        No recipes yet.
      </p>
      <div v-else class="grid grid-cols-[repeat(auto-fill,minmax(260px,1fr))] gap-4 max-sm:grid-cols-1">
        <RecipesRecipeCard
          v-for="recipe in recipes"
          :key="recipe.id"
          :recipe="recipe"
          @click="navigateTo(`/recipes/${recipe.id}`)"
        />
      </div>
    </DashboardPanel>

    <RecipesAddRecipeModal v-if="showModal" @added="handleAdded" @close="showModal = false" />
  </div>
</template>

<script setup lang="ts">
const { recipes, loading, error, isStale, start, stop, refresh } = useRecipes();
const showModal = ref(false);

onMounted(() => start());
onUnmounted(() => stop());

const handleAdded = (recipe: { id: string }) => {
  showModal.value = false;
  navigateTo(`/recipes/${recipe.id}`);
};
</script>
