<template>
  <div class="p-6">
    <SharedSkeletonBlock v-if="loading && !recipe" class="h-[120px]" />
    <SharedErrorBanner v-else-if="error" :message="error" :show-retry="true" @retry="fetchRecipe" />

    <template v-else-if="recipe">
      <header class="mb-8">
        <NuxtLink to="/recipes" class="inline-block mb-4 text-accent no-underline text-sm"
          >← Recipes</NuxtLink
        >
        <h1 class="font-display text-[32px] font-bold mb-3">{{ recipe.name }}</h1>
        <div class="flex gap-2 flex-wrap mb-3">
          <span
            v-for="tag in recipe.tags"
            :key="tag"
            class="bg-accent-subtle text-accent rounded-full px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.05em]"
            >{{ tag }}</span
          >
        </div>
        <p class="text-text-muted text-sm m-0">
          {{ recipe.prepMinutes + recipe.cookMinutes }} min total ({{ recipe.prepMinutes }} prep +
          {{ recipe.cookMinutes }} cook)
        </p>
      </header>

      <div class="flex flex-col gap-8">
        <section>
          <h2 class="font-display text-[20px] font-semibold mb-4 pb-2 border-b border-border">
            Instructions
          </h2>
          <p class="text-[15px] leading-[1.7] text-text-primary m-0">{{ recipe.instructions }}</p>
        </section>

        <section v-if="enrichedIngredients.length > 0">
          <h2 class="font-display text-[20px] font-semibold mb-4 pb-2 border-b border-border">
            Ingredients
          </h2>
          <ul class="list-none p-0 m-0 flex flex-col gap-3">
            <li
              v-for="ing in enrichedIngredients"
              :key="ing.id"
              class="flex items-center justify-between gap-3 p-3 bg-surface border border-border rounded-md"
            >
              <div class="flex flex-col">
                <span class="font-medium">{{ ing.ingredientName }}</span>
                <span v-if="ing.quantity" class="text-text-muted text-[13px]">
                  {{ ing.quantity }}{{ ing.unit }}
                </span>
              </div>
              <RecipesDetailIngredientStockBadge :is-in-stock="ing.isInStock ?? false" />
            </li>
          </ul>
        </section>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
const route = useRoute();
const { recipe, loading, error, enrichedIngredients, start, fetchRecipe, stop } = useRecipeDetail(
  route.params.id as string
);

onMounted(async () => {
  await start();
});

onUnmounted(() => stop());
</script>
