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
        <tr v-for="row in paginatedIngredients" :key="row.index" class="border-b border-border">
          <template v-if="mode === 'editable' && editingIndex === row.index">
            <td colspan="5" class="py-3">
              <div class="rounded-lg border border-border bg-background-muted p-3">
                <div class="grid gap-3 md:grid-cols-[1fr_90px_100px_auto] md:items-end">
                  <SharedFormField label="Ingredient" :error="editError">
                    <input
                      ref="ingredientNameInputRef"
                      v-model="row.ingredient.ingredientName"
                      class="input"
                      placeholder="e.g. Flour"
                      @keydown.enter.prevent="finishEditing(row.index)"
                    >
                  </SharedFormField>

                  <SharedFormField label="Qty">
                    <input
                      v-model.number="row.ingredient.quantity"
                      type="number"
                      min="0"
                      step="0.01"
                      class="input"
                      @keydown.enter.prevent="finishEditing(row.index)"
                    >
                  </SharedFormField>

                  <SharedFormField label="Unit">
                    <input
                      v-model="row.ingredient.unit"
                      class="input"
                      placeholder="g"
                      @keydown.enter.prevent="finishEditing(row.index)"
                    >
                  </SharedFormField>

                  <label class="flex items-center gap-2 text-sm text-text">
                    <input v-model="row.ingredient.isOptional" type="checkbox">
                    Optional
                  </label>
                </div>

                <div class="mt-3 flex justify-end gap-2">
                  <button type="button" class="btn btn-secondary text-xs" @click="removeIngredient(row.index)">Remove</button>
                  <button type="button" class="btn btn-primary text-xs" @click="finishEditing(row.index)">Done</button>
                </div>
              </div>
            </td>
          </template>

          <template v-else>
            <td class="py-2">{{ row.ingredient.ingredientName || "New ingredient" }}</td>
            <td class="py-2 text-right">{{ row.ingredient.quantity }}</td>
            <td class="py-2 text-right">{{ row.ingredient.unit }}</td>
            <td class="py-2 text-center">{{ row.ingredient.isOptional ? "Yes" : "—" }}</td>
            <td v-if="mode === 'readonly' && $slots['stock-badge']" class="py-2 text-center">
              <slot name="stock-badge" :ing="row.ingredient" />
            </td>
            <td v-if="mode === 'editable'" class="py-2 text-right">
              <div class="flex justify-end gap-2">
                <button type="button" class="btn btn-secondary text-xs" @click="editIngredient(row.index)">Edit</button>
                <button type="button" class="btn btn-secondary text-xs" @click="removeIngredient(row.index)">Remove</button>
              </div>
            </td>
          </template>
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
import { computed, nextTick, ref, watch } from "vue";
import type { RecipeIngredient } from "@/types";

type IngredientType = RecipeIngredient | Omit<RecipeIngredient, "id" | "recipeId" | "isInStock">;

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

const removeIngredient = (index: number) => {
  if (editingIndex.value === index) {
    editingIndex.value = null;
    editError.value = "";
  } else if (editingIndex.value !== null && index < editingIndex.value) {
    editingIndex.value -= 1;
  }

  emit("remove", index);
};

const currentPage = ref(1);
const editingIndex = ref<number | null>(props.mode === "editable" && props.ingredients.length > 0 ? 0 : null);
const editError = ref("");
const ingredientNameInputRef = ref<HTMLInputElement | readonly HTMLInputElement[]>();
const totalPages = computed(() => Math.max(1, Math.ceil(props.ingredients.length / props.pageSize)));

const pageForIndex = (index: number) => Math.floor(index / props.pageSize) + 1;

const editIngredient = async (index: number): Promise<void> => {
  editingIndex.value = index;
  editError.value = "";
  currentPage.value = pageForIndex(index);

  await nextTick();
  focusIngredientNameInput();
};

const focusIngredientNameInput = () => {
  const inputRef = ingredientNameInputRef.value;
  const input = Array.isArray(inputRef) ? inputRef[0] : inputRef;

  input?.focus();
};

const finishEditing = (index: number) => {
  const ingredient = props.ingredients[index];

  if (!ingredient?.ingredientName.trim()) {
    editError.value = "Ingredient name is required";
    return;
  }

  editError.value = "";
  editingIndex.value = null;
};

const paginatedIngredients = computed(() => {
  const start = (currentPage.value - 1) * props.pageSize;
  const end = start + props.pageSize;
  return props.ingredients.slice(start, end).map((ingredient, pageIndex) => ({
    ingredient,
    index: start + pageIndex,
  }));
});

watch(
  () => props.ingredients.length,
  () => {
    if (currentPage.value > totalPages.value) {
      currentPage.value = totalPages.value;
    }
  }
);

defineExpose({ editIngredient });
</script>
