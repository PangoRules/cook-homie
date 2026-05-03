<template>
  <SharedModal ref="modalRef" title="Add Recipe" @close="$emit('close')">
    <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
      <SharedFormField label="Name" :error="errors.name" required>
        <input v-model="form.name" class="input" placeholder="e.g. Banana Bread">
      </SharedFormField>

      <div class="grid grid-cols-2 gap-4">
        <SharedFormField label="Prep (min)">
          <input v-model.number="form.prepMinutes" type="number" min="0" class="input">
        </SharedFormField>
        <SharedFormField label="Cook (min)">
          <input v-model.number="form.cookMinutes" type="number" min="0" class="input">
        </SharedFormField>
      </div>

      <SharedFormField label="Instructions" :error="errors.instructions" required>
        <textarea v-model="form.instructions" class="input" rows="4" placeholder="Step by step..." />
      </SharedFormField>

      <SharedFormField label="Tags">
        <SharedTagInput v-model="form.tags" placeholder="Type tag, press Enter" />
      </SharedFormField>
      
      <SharedFormField label="Ingredients" :error="errors.ingredients" required>
        <div class="flex flex-col gap-2">
          <div 
            v-for="(ingredient, index) in form.ingredients" 
            :key="index" 
            class="grid grid-cols-[1fr_90px_100px_auto_auto] gap-2 max-md:grid-cols-1"
          >
            <input 
              v-model="ingredient.ingredientName" 
              class="input" 
              placeholder="Ingredient name"
            >
            <input 
              v-model.number="ingredient.quantity" 
              type="number" 
              class="input" 
              placeholder="Qty"
            >
            <input 
              v-model="ingredient.unit" 
              class="input" 
              placeholder="Unit"
            >
            <label class="flex items-center gap-2 text-sm text-text-secondary">
              <input v-model="ingredient.isOptional" type="checkbox">
              Optional
            </label>
            <button 
              type="button" 
              class="btn btn-secondary"
              @click="removeIngredient(index)"
            >
              Remove
            </button>
          </div>
          <button 
            type="button" 
            class="btn btn-secondary w-fit"
            @click="addIngredient"
          >
            Add Ingredient
          </button>
        </div>
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
import { computed, ref, reactive } from "vue";
import { useToast } from "@/composables/useToast";

const emit = defineEmits(["added", "close"]);
const modalRef = ref();

const form = reactive({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  instructions: "",
  tags: [] as string[],
  ingredients: [{ ingredientName: "", quantity: 1, unit: "", isOptional: false }],
});

const errors = reactive({ name: "", instructions: "", ingredients: "" });
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
  if (validIngredients.value.length === 0) { errors.ingredients = "At least one ingredient is required"; isValid = false; }
  else errors.ingredients = "";
  return isValid;
};

const resetForm = () => {
  form.name = ""; form.prepMinutes = 0; form.cookMinutes = 0; form.instructions = "";
  form.tags = []; form.ingredients = [emptyIngredient()];
  errors.name = ""; errors.instructions = ""; errors.ingredients = "";
};

const onCancel = () => {
  resetForm();
  modalRef.value.startClose();
};

const addIngredient = () => {
  form.ingredients.push(emptyIngredient());
};

const removeIngredient = (index: number) => {
  if (form.ingredients.length === 1) {
    form.ingredients[0] = emptyIngredient();
    return;
  }
  form.ingredients.splice(index, 1);
};

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
