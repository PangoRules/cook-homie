<template>
  <div v-if="isOpen" class="modal-overlay" @click="closeModal">
    <div class="modal-content" @click.stop>
      <h2>Add New Inventory Item</h2>
      
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="name">Item Name *</label>
          <input
            id="name"
            v-model="form.name"
            type="text"
            required
            :disabled="isSubmitting"
          />
          <span v-if="errors.name" class="error">{{ errors.name }}</span>
        </div>

        <div class="form-group">
          <label for="quantity">Quantity *</label>
          <input
            id="quantity"
            v-model.number="form.quantity"
            type="number"
            min="0"
            required
            :disabled="isSubmitting"
          />
          <span v-if="errors.quantity" class="error">{{ errors.quantity }}</span>
        </div>

        <div class="form-group">
          <label for="unit">Unit</label>
          <select
            id="unit"
            v-model="form.unit"
            :disabled="isSubmitting"
          >
            <option value="pieces">pieces</option>
            <option value="grams">grams</option>
            <option value="kilograms">kilograms</option>
            <option value="milliliters">milliliters</option>
            <option value="liters">liters</option>
            <option value="boxes">boxes</option>
            <option value="bottles">bottles</option>
            <option value="packages">packages</option>
          </select>
        </div>

        <div class="form-group">
          <label for="expiryDate">Expiry Date</label>
          <input
            id="expiryDate"
            v-model="form.expiryDate"
            type="date"
            :disabled="isSubmitting"
          />
        </div>

        <div class="form-group">
          <label for="tags">Tags</label>
          <input
            id="tags"
            v-model="form.tags"
            type="text"
            placeholder="Enter tags separated by commas"
            :disabled="isSubmitting"
          />
        </div>

        <div class="modal-actions">
          <button type="button" @click="closeModal" :disabled="isSubmitting">
            Cancel
          </button>
          <button type="submit" :disabled="isSubmitting">
            {{ isSubmitting ? 'Adding...' : 'Add Item' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue';

const props = defineProps<{
  isOpen: boolean;
}>();

const emit = defineEmits<{
  (e: 'close'): void;
  (e: 'item-added'): void;
}>();

const form = reactive({
  name: '',
  quantity: 1,
  unit: 'pieces',
  expiryDate: '',
  tags: ''
});

const errors = reactive({
  name: '',
  quantity: ''
});

const isSubmitting = ref(false);

// Reset form when modal opens
watch(() => props.isOpen, (isOpen) => {
  if (isOpen) {
    resetForm();
  }
});

const resetForm = () => {
  form.name = '';
  form.quantity = 1;
  form.unit = 'pieces';
  form.expiryDate = '';
  form.tags = '';
  errors.name = '';
  errors.quantity = '';
};

const validateForm = () => {
  let isValid = true;
  
  if (!form.name.trim()) {
    errors.name = 'Item name is required';
    isValid = false;
  } else {
    errors.name = '';
  }
  
  if (form.quantity <= 0) {
    errors.quantity = 'Quantity must be greater than 0';
    isValid = false;
  } else {
    errors.quantity = '';
  }
  
  return isValid;
};

const handleSubmit = async () => {
  if (!validateForm()) {
    return;
  }

  isSubmitting.value = true;
  
  try {
    const payload = {
      name: form.name.trim(),
      quantity: form.quantity,
      unit: form.unit,
      expiresAt: form.expiryDate || null,
      tags: form.tags.split(',').map(tag => tag.trim()).filter(tag => tag)
    };

    // In a real implementation, this would call the API
    // await useAddItemForm().addInventoryItem(payload);
    
    // For now, just emit the event to simulate
    emit('item-added');
    closeModal();
  } catch (error) {
    // Error handling would be implemented here
    console.error('Failed to add item:', error);
  } finally {
    isSubmitting.value = false;
  }
};

const closeModal = () => {
  emit('close');
};
</script>

<style scoped>
.modal-overlay {
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

.modal-content {
  background-color: white;
  padding: 20px;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

input,
select {
  width: 100%;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-sizing: border-box;
}

.error {
  color: red;
  font-size: 0.9em;
  margin-top: 5px;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

.modal-actions button {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.modal-actions button[type="submit"] {
  background-color: #007bff;
  color: white;
}

.modal-actions button[type="button"] {
  background-color: #6c757d;
  color: white;
}

.modal-actions button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>