<template>
  <div class="modal-backdrop" @click.self="$emit('close')">
    <div class="modal">
      <header class="modal__header">
        <h2 class="modal__title">Add Recipe</h2>
        <button class="modal__close" @click="$emit('close')">×</button>
      </header>

      <form class="modal__form" @submit.prevent="handleSubmit">
        <div class="field">
          <label class="field__label">Name *</label>
          <input v-model="form.name" class="field__input" placeholder="e.g. Banana Bread" required />
          <span v-if="errors.name" class="field__error">{{ errors.name }}</span>
        </div>

        <div class="field-row">
          <div class="field">
            <label class="field__label">Prep (min)</label>
            <input v-model.number="form.prepMinutes" type="number" min="0" class="field__input" />
          </div>
          <div class="field">
            <label class="field__label">Cook (min)</label>
            <input v-model.number="form.cookMinutes" type="number" min="0" class="field__input" />
          </div>
        </div>

        <div class="field">
          <label class="field__label">Instructions *</label>
          <textarea v-model="form.instructions" class="field__input" rows="4" placeholder="Step by step..." required />
          <span v-if="errors.instructions" class="field__error">{{ errors.instructions }}</span>
        </div>

        <div class="field">
          <label class="field__label">Tags (comma separated)</label>
          <input v-model="tagsInput" class="field__input" placeholder="quick, vegetarian" />
        </div>

        <div class="modal__actions">
          <button type="button" class="btn-secondary" @click="$emit('close')">Cancel</button>
          <button type="submit" class="btn-primary" :disabled="submitting">
            {{ submitting ? "Adding…" : "Add Recipe" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { useToast } from "@/composables/useToast";

const emit = defineEmits(["added", "close"]);

const form = reactive({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  instructions: "",
  tags: [] as string[],
});

const tagsInput = ref("");
const errors = reactive({
  name: "",
  instructions: "",
});

const submitting = ref(false);

const validate = () => {
  let isValid = true;
  if (!form.name.trim()) {
    errors.name = "Recipe name is required";
    isValid = false;
  } else {
    errors.name = "";
  }

  if (!form.instructions.trim()) {
    errors.instructions = "Instructions are required";
    isValid = false;
  } else {
    errors.instructions = "";
  }

  return isValid;
};

const resetForm = () => {
  form.name = "";
  form.prepMinutes = 0;
  form.cookMinutes = 0;
  form.instructions = "";
  tagsInput.value = "";
  errors.name = "";
  errors.instructions = "";
};

const handleSubmit = async () => {
  if (!validate()) return;

  submitting.value = true;

  try {
    const toast = useToast();
    const response = await $fetch("/api/recipes", {
      method: "POST",
      body: {
        ...form,
        tags: tagsInput.value
          .split(",")
          .map((tag) => tag.trim())
          .filter((tag) => tag.length > 0),
      },
    });

    emit("added", response);
    resetForm();
    toast.pushSuccess("Recipe added successfully!");
  } catch (error) {
    const toast = useToast();
    toast.pushError("Failed to add recipe");
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal {
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal__header {
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal__title {
  font-family: var(--font-display);
  font-size: 1.25rem;
  margin: 0;
}

.modal__close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
}

.modal__form {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.field {
  display: flex;
  flex-direction: column;
}

.field__label {
  font-weight: 600;
  margin-bottom: 0.25rem;
  color: #333;
}

.field__input {
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 1rem;
}

.field__input:focus {
  outline: none;
  border-color: #007bff;
}

.field__error {
  color: #e74c3c;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.field-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.modal__actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  padding-top: 1rem;
  border-top: 1px solid #eee;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.2s;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background-color: #545b62;
}
</style>