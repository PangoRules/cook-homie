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
import type { AddRecipePayload } from "~/types";

const emit = defineEmits(["added", "close"]);

const form = reactive<AddRecipePayload>({
  name: "",
  instructions: "",
  prepMinutes: 0,
  cookMinutes: 0,
  tags: []
});

const tagsInput = ref("");
const errors = reactive({ name: "", instructions: "" });
const submitting = ref(false);
const { pushSuccess, pushError } = useToast();

const validate = () => {
  errors.name = "";
  errors.instructions = "";
  if (!form.name.trim()) errors.name = "Name is required";
  if (!form.instructions.trim()) errors.instructions = "Instructions are required";
  return !errors.name && !errors.instructions;
};

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    form.tags = tagsInput.value.split(",").map(t => t.trim()).filter(Boolean);
    const created = await $fetch("/api/recipes", { method: "POST", body: form });
    emit("added", created);
    pushSuccess(`"${form.name}" added!`);
  } catch (err) {
    pushError(err instanceof Error ? err : new Error("Failed to add recipe"));
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}

.modal {
  background: var(--color-surface);
  border-radius: var(--radius-lg);
  width: 90%;
  max-width: 480px;
  box-shadow: var(--shadow-lg);
  overflow: hidden;
}

.modal__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-4) var(--space-5);
  border-bottom: 1px solid var(--color-border);
}

.modal__title {
  font-family: var(--font-display);
  font-size: 18px;
  font-weight: 600;
  margin: 0;
}

.modal__close { background: none; border: none; font-size: 22px; cursor: pointer; color: var(--color-text-muted); }

.modal__form { padding: var(--space-5); display: flex; flex-direction: column; gap: var(--space-4); }

.field { display: flex; flex-direction: column; gap: var(--space-1); }
.field__label { font-size: 13px; font-weight: 600; color: var(--color-text-secondary); }
.field__input {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-3);
  font-size: 14px;
  font-family: var(--font-body);
  background: var(--color-bg);
  color: var(--color-text-primary);
  width: 100%;
  box-sizing: border-box;
}
.field__input:focus { outline: none; border-color: var(--color-accent); }
.field__error { font-size: 12px; color: var(--color-error); }

.field-row { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-3); }

.modal__actions { display: flex; justify-content: flex-end; gap: var(--space-3); margin-top: var(--space-2); }

.btn-primary {
  background: var(--color-accent);
  color: white;
  border: none;
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-5);
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background var(--transition-fast);
}
.btn-primary:hover:not(:disabled) { background: var(--color-accent-hover); }
.btn-primary:disabled { opacity: 0.5; cursor: not-allowed; }

.btn-secondary {
  background: none;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-5);
  font-size: 14px;
  cursor: pointer;
  color: var(--color-text-secondary);
}
</style>