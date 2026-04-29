<template>
  <div class="recipes-page">
    <header class="recipes-page__header">
      <h1 class="recipes-page__title">Recipes</h1>
      <button class="btn-add" @click="showModal = true">+ Add Recipe</button>
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
      <div v-if="loading && recipes.length === 0" class="recipes-grid">
        <SkeletonBlock v-for="i in 3" :key="i" height="120px" />
      </div>
      <div v-else-if="error && recipes.length === 0">
        <ErrorBanner :message="error" show-retry @retry="refresh" />
      </div>
      <div v-else-if="recipes.length === 0" class="empty-state">
        <p>No recipes yet.</p>
      </div>
      <div v-else class="recipes-grid">
        <RecipeCard
          v-for="recipe in recipes"
          :key="recipe.id"
          :recipe="recipe"
          @click="navigateTo(`/recipes/${recipe.id}`)"
        />
      </div>
    </DashboardPanel>

    <AddRecipeModal v-if="showModal" @added="handleAdded" @close="showModal = false" />
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

<style scoped>
.recipes-page { max-width: 960px; margin: 0 auto; }

.recipes-page__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-6);
}

.recipes-page__title {
  font-family: var(--font-display);
  font-size: 28px;
  font-weight: 700;
  margin: 0;
}

.btn-add {
  background: var(--color-accent);
  color: white;
  border: none;
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-5);
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
}

.recipes-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: var(--space-4);
}

.empty-state {
  text-align: center;
  padding: var(--space-10);
  color: var(--color-text-muted);
}

@media (max-width: 480px) {
  .recipes-page__header { flex-direction: column; align-items: flex-start; gap: var(--space-3); }
  .recipes-grid { grid-template-columns: 1fr; }
}
</style>