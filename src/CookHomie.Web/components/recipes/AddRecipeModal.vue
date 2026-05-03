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
      
      <SharedFormField label="Ingredients">
        <div class="flex flex-col gap-2">
          <div 
            v-for="(ingredient, index) in form.ingredients" 
            :key="index" 
            class="flex gap-2"
          >
            <input 
              v-model="ingredient.ingredientName" 
              class="input flex-1" 
              placeholder="Ingredient name"
            >
            <input 
              v-model.number="ingredient.quantity" 
              type="number" 
              class="input w-20" 
              placeholder="Qty"
            >
            <input 
              v-model="ingredient.unit" 
              class="input w-20" 
              placeholder="Unit"
            >
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
import { ref, reactive } from "vue";
import { useToast } from "@/composables/useToast";

const emit = defineEmits(["added", "close"]);
const modalRef = ref();

const form = reactive({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  instructions: "",
  tags: [] as string[],
  ingredients: [] as { ingredientName: string; quantity: number; unit: string }[],
});

const errors = reactive({ name: "", instructions: "" });
const submitting = ref(false);

const validate = () => {
  let isValid = true;
  if (!form.name.trim()) { errors.name = "Recipe name is required"; isValid = false; }
  else errors.name = "";
  if (!form.instructions.trim()) { errors.instructions = "Instructions are required"; isValid = false; }
  else errors.instructions = "";
  return isValid;
};

const resetForm = () => {
  form.name = ""; form.prepMinutes = 0; form.cookMinutes = 0; form.instructions = "";
  form.tags = []; form.ingredients = [];
  errors.name = ""; errors.instructions = "";
};

const onCancel = () => {
  resetForm();
  modalRef.value.startClose();
};

const addIngredient = () => {
  form.ingredients.push({ ingredientName: "", quantity: 0, unit: "" });
};

const removeIngredient = (index: number) => {
  form.ingredients.splice(index, 1);
};

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    const toast = useToast();
    const response = await $fetch("/api/recipes", {
      method: "POST",
      body: { ...form },
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
