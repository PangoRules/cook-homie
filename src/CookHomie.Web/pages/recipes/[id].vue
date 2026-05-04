<template>
  <div class="p-6">
    <SharedSkeletonBlock v-if="loading && !recipe" class="h-[120px]" />
    <SharedErrorBanner v-else-if="error" :message="error" :show-retry="true" @retry="fetchRecipe" />

    <template v-else-if="recipe">
      <header class="mb-8">
        <NuxtLink to="/recipes" class="inline-block mb-4 text-accent no-underline text-sm"
          >← Recipes</NuxtLink
        >
        <h1 class="font-display text-[32px] font-bold mb-3">
          {{ editMode === "meta" ? editBuffer.name : recipe.name }}
        </h1>
        <div class="flex gap-2 flex-wrap mb-3">
          <template v-if="editMode !== 'meta'">
            <span
              v-for="tag in recipe.tags"
              :key="tag"
              class="bg-accent-subtle text-accent rounded-full px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.05em]"
              >{{ tag }}</span
            >
          </template>
          <template v-else>
            <input
              v-for="(tag, i) in editBuffer.tags ?? []"
              :key="i"
              v-model="editBuffer.tags![i]"
              class="input text-[11px] w-[100px]"
              placeholder="tag"
            >
            <button
              type="button"
              class="btn btn-secondary text-[11px]"
              @click="editBuffer.tags ? editBuffer.tags.push('') : null">+ Tag</button>
          </template>
        </div>
        <p class="text-text-muted text-sm m-0">
          <template v-if="editMode !== 'meta'">
            {{ recipe.prepMinutes + recipe.cookMinutes }} min total ({{ recipe.prepMinutes }} prep +
            {{ recipe.cookMinutes }} cook)
          </template>
          <template v-else>
            <input v-model.number="editBuffer.prepMinutes" type="number" min="0" class="input w-[60px] mr-2">prep +
            <input v-model.number="editBuffer.cookMinutes" type="number" min="0" class="input w-[60px] mx-2">cook min
          </template>
        </p>
      </header>

      <div class="flex flex-col gap-8">
        <!-- META SECTION -->
        <section>
          <div class="flex items-center justify-between mb-3">
            <h2 class="font-display text-[20px] font-semibold">Recipe Info</h2>
            <button v-if="editMode === 'none'" class="btn btn-secondary text-xs" @click="startEditMeta">
              Edit
            </button>
          </div>
          <div v-if="editMode !== 'meta'" class="flex gap-3 flex-wrap">
            <span class="text-text-muted text-sm">Tags: {{ recipe.tags.join(", ") || "none" }}</span>
            <span class="text-text-muted text-sm">|</span>
            <span class="text-text-muted text-sm">Prep: {{ recipe.prepMinutes }}m</span>
            <span class="text-text-muted text-sm">|</span>
            <span class="text-text-muted text-sm">Cook: {{ recipe.cookMinutes }}m</span>
          </div>
          <div v-else class="flex flex-col gap-3">
            <SharedFormField label="Name">
              <input v-model="editBuffer.name" class="input"></input>
            </SharedFormField>
            <SharedFormField label="Tags (comma separated)">
            <input
              :value="editBuffer.tags?.join(', ')"
              class="input"
              @blur="editBuffer.tags = ($event.target as HTMLInputElement).value.split(',').map(t => t.trim()).filter(Boolean) ?? []"
            >
            </SharedFormField>
            <div class="flex gap-4">
              <SharedFormField label="Prep (min)">
                <input v-model.number="editBuffer.prepMinutes" type="number" min="0" class="input w-[80px]">
              </SharedFormField>
              <SharedFormField label="Cook (min)">
                <input v-model.number="editBuffer.cookMinutes" type="number" min="0" class="input w-[80px]">
              </SharedFormField>
            </div>
            <div class="flex gap-2">
              <SharedButton variant="primary" @click="saveEdit">Save</SharedButton>
              <SharedButton variant="secondary" @click="cancelEdit">Cancel</SharedButton>
            </div>
          </div>
        </section>

        <!-- INSTRUCTIONS SECTION -->
        <section>
          <div class="flex items-center justify-between mb-4 pb-2 border-b border-border">
            <h2 class="font-display text-[20px] font-semibold">Instructions</h2>
            <button v-if="editMode === 'none'" class="btn btn-secondary text-xs" @click="startEditInstructions">
              Edit
            </button>
          </div>
          <div v-if="editMode !== 'instructions'">
            <p class="text-[15px] leading-[1.7] text-text-primary m-0">{{ recipe.instructions }}</p>
          </div>
          <div v-else class="flex flex-col gap-3">
            <SharedFormField label="Instructions">
              <textarea
                v-model="editBuffer.instructions"
                class="input min-h-[120px]"
                rows="5"
              />
            </SharedFormField>
            <div class="flex gap-2">
              <SharedButton variant="primary" @click="saveEdit">Save</SharedButton>
              <SharedButton variant="secondary" @click="cancelEdit">Cancel</SharedButton>
            </div>
          </div>
        </section>

        <!-- INGREDIENTS SECTION -->
        <section v-if="enrichedIngredients.length > 0">
          <div class="flex items-center justify-between mb-4 pb-2 border-b border-border">
            <h2 class="font-display text-[20px] font-semibold">Ingredients</h2>
            <button v-if="editMode === 'none'" class="btn btn-secondary text-xs" @click="startEditIngredients">
              Edit
            </button>
          </div>
          <IngredientTable
            v-if="editMode === 'ingredients'"
            v-model:ingredients="editIngredients"
            mode="editable"
          />
          <IngredientTable
            v-else
            :ingredients="enrichedIngredients"
            mode="readonly"
          >
            <template #stock-badge="{ ing }">
              <RecipesDetailIngredientStockBadge :is-in-stock="ing.isInStock ?? false" />
              <button
                v-if="!(ing.isInStock ?? false)"
                class="ml-2 text-xs text-accent hover:underline"
                @click="addMissingToShoppingList(ing.ingredientName)"
              >
                + Add to list
              </button>
            </template>
          </IngredientTable>
          <div v-if="editMode === 'ingredients'" class="flex gap-2 mt-3">
            <SharedButton variant="primary" @click="saveIngredientsEdit">Save</SharedButton>
            <SharedButton variant="secondary" @click="cancelEdit">Cancel</SharedButton>
          </div>
        </section>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import type { Recipe, RecipeIngredient } from "~/types";

const route = useRoute();
const { recipe, loading, error, enrichedIngredients, fetchRecipe, updateRecipe, start, stop } = useRecipeDetail(
  route.params.id as string
);

const editMode = ref<"none" | "meta" | "instructions" | "ingredients">("none");
const editBuffer = ref<Partial<Recipe>>({});
const editIngredients = ref<RecipeIngredient[]>([]);

const startEditMeta = () => {
  editBuffer.value = {
    name: recipe.value?.name,
    tags: [...(recipe.value?.tags ?? [])],
    prepMinutes: recipe.value?.prepMinutes,
    cookMinutes: recipe.value?.cookMinutes,
  };
  editMode.value = "meta";
};

const startEditInstructions = () => {
  editBuffer.value = { instructions: recipe.value?.instructions ?? "" };
  editMode.value = "instructions";
};

const startEditIngredients = () => {
  editIngredients.value = recipe.value?.ingredients.map(ing => ({ ...ing })) ?? [];
  editMode.value = "ingredients";
};

const cancelEdit = () => {
  editMode.value = "none";
};

const saveEdit = async () => {
  await updateRecipe(editBuffer.value);
  editMode.value = "none";
  useToast().pushSuccess("Recipe updated");
};

const saveIngredientsEdit = async () => {
  await updateRecipe({ ingredients: editIngredients.value });
  editMode.value = "none";
  useToast().pushSuccess("Recipe ingredients updated");
};

const addMissingToShoppingList = async (ingredientName: string) => {
  const { addBulk, items } = useShoppingList();
  const alreadyOnList = items.value.some(
    (s) => s.name.toLowerCase() === ingredientName.toLowerCase() && !s.isBought
  );
  if (alreadyOnList) {
    useToast().pushSuccess("Already on shopping list");
    return;
  }
  try {
    await addBulk([ingredientName]);
    useToast().pushSuccess(`"${ingredientName}" added to shopping list`);
  } catch {
    useToast().pushError("Failed to add to shopping list");
  }
};

onMounted(async () => {
  await start();
});

onUnmounted(() => stop());
</script>