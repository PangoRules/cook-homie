<template>
  <div class="flex flex-col gap-2">
    <table class="w-full text-sm">
      <thead>
        <tr class="border-b border-border">
          <th class="text-left text-text-muted font-medium">Ingredient</th>
          <th class="text-right text-text-muted font-medium w-[80px]">Qty</th>
          <th class="text-right text-text-muted font-medium w-[60px]">Unit</th>
          <th class="text-center text-text-muted font-medium w-[60px]">Optional</th>
          <th v-if="mode === 'readonly'" class="text-center text-text-muted font-medium w-[80px]">Status</th>
          <th v-if="mode === 'editable'" class="w-[60px]" />
        </tr>
      </thead>
      <tbody>
        <tr v-for="(ing, i) in paginatedIngredients" :key="i" class="border-b border-border">
          <td class="py-2">{{ ing.ingredientName }}</td>
          <td class="py-2 text-right">{{ ing.quantity }}</td>
          <td class="py-2 text-right">{{ ing.unit }}</td>
          <td class="py-2 text-center">{{ ing.isOptional ? "Yes" : "—" }}</td>
          <td v-if="mode === 'readonly' && $slots['stock-badge']" class="py-2 text-center">
            <slot name="stock-badge" :ing="ing" />
          </td>
          <td v-if="mode === 'editable'" class="py-2 text-right">
            <button type="button" class="btn btn-secondary text-xs" @click="removeIngredient(i)">Remove</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Pagination controls -->
    <div class="flex items-center justify-between">
      <span class="text-xs text-text-muted">Page {{ currentPage }} of {{ totalPages }}</span>
      <div class="flex gap-2">
        <SharedButton size="small" variant="secondary" :disabled="currentPage === 1" @click="currentPage--">
          Prev
        </SharedButton>
        <SharedButton size="small" variant="secondary" :disabled="currentPage === totalPages" @click="currentPage++">
          Next
        </SharedButton>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import type { RecipeIngredient } from "@/types";

type IngredientType = RecipeIngredient | Omit<RecipeIngredient, 'id' | 'recipeId' | 'isInStock'>;

interface Props {
  ingredients: IngredientType[];
  mode?: "readonly" | "editable";
  pageSize?: number;
}

const props = withDefaults(defineProps<Props>(), {
  mode: "readonly",
  pageSize: 5,
});

const emit = defineEmits<{
  (e: "remove", index: number): void;
}>();

// This function handles removal of ingredients from an editable ingredient table
const removeIngredient = (index: number) => {
  emit("remove", index);
};

const currentPage = ref(1);
const totalPages = computed(() => Math.ceil(props.ingredients.length / props.pageSize));

const paginatedIngredients = computed(() => {
  const start = (currentPage.value - 1) * props.pageSize;
  const end = start + props.pageSize;
  return props.ingredients.slice(start, end);
});
</script>