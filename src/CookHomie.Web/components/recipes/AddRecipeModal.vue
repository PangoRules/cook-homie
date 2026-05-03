<template>
  <SharedModal ref="modalRef" title="Add Recipe" @close="$emit('close')">
    <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
      <SharedFormField label="Name" :error="errors.name" required>
        <input v-model="form.name" class="input" placeholder="e.g. Banana Bread">
      </SharedFormField>

      <div class="grid grid-cols-2 gap-4">
        <SharedFormField label="Prep (min)" :error="errors.prepMinutes" required>
          <input v-model.number="form.prepMinutes" type="number" min="1" class="input">
        </SharedFormField>
        <SharedFormField label="Cook (min)" :error="errors.cookMinutes" required>
          <input v-model.number="form.cookMinutes" type="number" min="1" class="input">
        </SharedFormField>
      </div>

      <SharedFormField label="Instructions" :error="errors.instructions" required>
        <textarea v-model="form.instructions" class="input" rows="4" placeholder="Step by step..." />
      </SharedFormField>

      <SharedFormField label="Tags">
        <SharedTagInput v-model="form.tags" placeholder="Type tag, press Enter" />
      </SharedFormField>
      
      <SharedFormField label="Ingredients" :error="errors.ingredients" required>
        <IngredientTable
          ref="ingredientTableRef"
          v-model:ingredients="form.ingredients"
          mode="editable"
          :page-size="5"
          :start-editing-first-row="true"
          @remove="removeIngredient"
          @cancel-add="cancelIngredientAdd"
        />
        <button type="button" class="btn btn-secondary w-fit mt-2" @click="addIngredient">
          + Add Ingredient
        </button>
      </SharedFormField>
    </form>

    <template #footer>
      <SharedButton variant="secondary" @click="onCancel">Cancel</SharedButton>
      <SharedButton variant="primary" :disabled="submitting" @click="handleSubmit">
        {{ submitting ? "Adding…" : "Add Recipe" }}
      </SharedButton>
    </template>
  </SharedModal>
</template>

<script setup lang="ts">
import { computed, nextTick, reactive, ref } from "vue";
import { useToast } from "@/composables/useToast";
import IngredientTable from "@/components/shared/IngredientTable.vue";

const emit = defineEmits(["added", "close"]);
const modalRef = ref();
const ingredientTableRef = ref<{ editIngredient: (index: number) => Promise<void> }>();

const form = reactive({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  instructions: "",
  tags: [] as string[],
  ingredients: [{ ingredientName: "", quantity: 1, unit: "", isOptional: false }],
});

const errors = reactive({ name: "", instructions: "", ingredients: "", prepMinutes: "", cookMinutes: "" });
const submitting = ref(false);

const emptyIngredient = () => ({ ingredientName: "", quantity: 1, unit: "", isOptional: false });

const validIngredients = computed(() =>
  form.ingredients.filter((ingredient) => ingredient.ingredientName.trim())
);

const validate = () => {
  let isValid = true;
  if (!form.name.trim()) { errors.name = "Recipe name is required"; isValid = false; }
  else errors.name = "";
  if (!form.instructions.trim()) { errors.instructions = "Instructions are required"; isValid = false; }
  else errors.instructions = "";
  if (form.prepMinutes < 1) { errors.prepMinutes = "Must be at least 1 minute"; isValid = false; }
  else errors.prepMinutes = "";
  if (form.cookMinutes < 1) { errors.cookMinutes = "Must be at least 1 minute"; isValid = false; }
  else errors.cookMinutes = "";
  if (validIngredients.value.length === 0) { errors.ingredients = "At least one ingredient is required"; isValid = false; }
  else errors.ingredients = "";
  return isValid;
};

const resetForm = () => {
  form.name = ""; form.prepMinutes = 0; form.cookMinutes = 0; form.instructions = "";
  form.tags = []; form.ingredients = [emptyIngredient()];
  errors.name = ""; errors.instructions = ""; errors.ingredients = "";
  errors.prepMinutes = ""; errors.cookMinutes = "";
};

const onCancel = () => {
  resetForm();
  modalRef.value.startClose();
};

const addIngredient = async () => {
  form.ingredients.push(emptyIngredient());
  await nextTick();
  await ingredientTableRef.value?.editIngredient(form.ingredients.length - 1);
};

const removeIngredient = (index: number) => {
  if (form.ingredients.length === 1) {
    form.ingredients = [];
    return;
  }
  form.ingredients.splice(index, 1);
};

const cancelIngredientAdd = (index: number) => {
  if (form.ingredients.length === 1) {
    form.ingredients = [];
    return;
  }
  form.ingredients.splice(index, 1);
};

defineExpose({ removeIngredient });

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    const toast = useToast();
    const response = await $fetch("/api/recipes", {
      method: "POST",
      body: {
        name: form.name.trim(),
        prepMinutes: Number(form.prepMinutes) || 0,
        cookMinutes: Number(form.cookMinutes) || 0,
        instructions: form.instructions.trim(),
        tags: form.tags,
        ingredients: validIngredients.value.map((ingredient) => ({
          ingredientName: ingredient.ingredientName.trim(),
          quantity: Number(ingredient.quantity) || 0,
          unit: ingredient.unit.trim(),
          isOptional: ingredient.isOptional,
        })),
      },
    });
    emit("added", response);
    resetForm();
    toast.pushSuccess("Recipe added successfully!");
    modalRef.value.startClose();
  } catch {
    useToast().pushError("Failed to add recipe");
  } finally {
    submitting.value = false;
  }
};
</script>
