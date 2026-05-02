<template>
  <SharedModal ref="modalRef" title="Add Inventory Item" @close="$emit('close')">
    <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
      <SharedFormField label="Name" :error="errors.name" required>
        <input v-model="form.name" class="input" placeholder="e.g. Milk" />
      </SharedFormField>

      <div class="grid grid-cols-2 gap-4">
        <SharedFormField label="Category" :error="errors.category" required>
          <input v-model="form.category" class="input" placeholder="e.g. dairy" />
        </SharedFormField>
        <SharedFormField label="Location" :error="errors.location" required>
          <select v-model="form.location" class="input">
            <!-- TODO: These need to be pulled from the backend. -->
            <option value="">Select...</option>
            <option value="Pantry">Pantry</option>
            <option value="Fridge">Fridge</option>
            <option value="Freezer">Freezer</option>
            <option value="Spices">Spices</option>
          </select>
        </SharedFormField>
      </div>

      <div class="grid grid-cols-2 gap-4">
        <SharedFormField label="Quantity" :error="errors.quantity" required>
          <input v-model.number="form.quantity" type="number" min="0" step="1" class="input" />
        </SharedFormField>
        <SharedFormField label="Unit">
          <input v-model="form.unit" class="input" placeholder="e.g. liters" />
        </SharedFormField>
      </div>

      <SharedFormField label="Expires At">
        <input v-model="form.expiresAt" type="date" class="input" />
      </SharedFormField>

      <SharedFormField label="Notes">
        <textarea v-model="form.notes" class="input" rows="2" placeholder="Optional notes..." />
      </SharedFormField>
    </form>

    <template #footer>
      <SharedButton variant="secondary" @click="onCancel">Cancel</SharedButton>
      <SharedButton variant="primary" :disabled="submitting" @click="handleSubmit">
        {{ submitting ? "Adding…" : "Add Item" }}
      </SharedButton>
    </template>
  </SharedModal>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import type { AddInventoryItemPayload } from "~/types";
import { useToast } from "~/composables/useToast";
import { useInventory } from "~/composables/useInventory";

const emit = defineEmits(["added", "close"]);
const modalRef = ref();
const { pushSuccess, pushError } = useToast();

const form = reactive({
  name: "",
  category: "",
  location: "",
  quantity: 1,
  unit: "",
  isOpened: false,
  expiresAt: "",
  notes: "",
});

const errors = reactive({ name: "", category: "", location: "", quantity: "" });
const submitting = ref(false);

const validate = () => {
  let valid = true;
  errors.name = "";
  errors.category = "";
  errors.location = "";
  errors.quantity = "";
  if (!form.name.trim()) {
    errors.name = "Name is required";
    valid = false;
  }
  if (!form.category.trim()) {
    errors.category = "Category is required";
    valid = false;
  }
  if (!form.location) {
    errors.location = "Location is required";
    valid = false;
  }
  if (form.quantity <= 0) {
    errors.quantity = "Quantity must be greater than 0";
    valid = false;
  }
  return valid;
};

const resetForm = () => {
  form.name = "";
  form.category = "";
  form.location = "";
  form.quantity = 1;
  form.unit = "";
  form.isOpened = false;
  form.expiresAt = "";
  form.notes = "";
  errors.name = "";
  errors.category = "";
  errors.location = "";
  errors.quantity = "";
};

const onCancel = () => {
  resetForm();
  modalRef.value.startClose();
};

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    const { addInventoryItem } = useInventory();
    const payload: AddInventoryItemPayload = {
      name: form.name,
      category: form.category,
      location: form.location,
      quantity: form.quantity,
      unit: form.unit,
      isOpened: form.isOpened,
      expiresAt: form.expiresAt || undefined,
      notes: form.notes || undefined,
    };
    const created = await addInventoryItem(payload);
    emit("added", created);
    resetForm();
    pushSuccess(`"${form.name}" added to inventory!`);
    modalRef.value.startClose();
  } catch (err) {
    pushError(err instanceof Error ? err : new Error("Failed to add item"));
  } finally {
    submitting.value = false;
  }
};
</script>
