<template>
  <div class="inventory-page">
    <div class="flex justify-between items-start gap-4 mb-6">
      <h1 class="text-2xl font-bold">Inventory</h1>
      <SharedButton @click="openAddItemModal">Add Item</SharedButton>
    </div>

    <SharedStaleIndicator v-if="isStale" class="mb-4" />

    <!-- Loading skeleton -->
    <div v-if="loading && items.length === 0" class="space-y-3">
      <SharedSkeletonBlock width="100%" height="40px" />
      <SharedSkeletonBlock width="100%" height="40px" />
      <SharedSkeletonBlock width="100%" height="40px" />
    </div>

    <!-- Error state -->
    <SharedErrorBanner
      v-else-if="error && items.length === 0"
      :message="error"
      class="mb-4"
      @retry="retryLoad"
    />

    <!-- Empty state -->
    <div v-else-if="items.length === 0" class="text-center py-12">
      <p class="text-text-secondary mb-4">Your inventory is empty</p>
      <SharedButton variant="primary" @click="openAddItemModal">Add Your First Item</SharedButton>
    </div>

    <!-- Inventory items -->
    <div v-else class="space-y-3">
      <div
        v-for="item in items"
        :key="item.id"
        class="flex items-center justify-between p-4 bg-surface border border-border rounded-lg"
      >
        <div>
          <h3 class="font-medium">{{ item.name }}</h3>
          <p class="text-sm text-text-secondary">
            {{ item.quantity }} {{ item.unit }} {{ item.expiresAt ? ` · Expires ${formatDate(item.expiresAt)}` : '' }}
          </p>
        </div>
        <div class="flex gap-2">
          <SharedButton variant="secondary" size="sm">Edit</SharedButton>
          <SharedButton variant="secondary" size="sm">Remove</SharedButton>
        </div>
      </div>
    </div>

<!-- Add Item Modal -->
    <SharedModal
      v-if="showAddItemModal"
      title="Add Inventory Item"
      @close="closeAddItemModal"
    >
      <form class="flex flex-col gap-4" @submit.prevent="handleAddItem">
        <SharedFormField label="Name" :error="errors.name" required>
          <input
            v-model="form.name"
            class="w-full p-2 border border-border rounded-md bg-surface"
            placeholder="e.g. Apples"
          >
        </SharedFormField>

        <div class="grid grid-cols-2 gap-4">
          <SharedFormField label="Quantity">
            <input
              v-model.number="form.quantity"
              type="number"
              min="0"
              class="w-full p-2 border border-border rounded-md bg-surface"
            >
          </SharedFormField>
          <SharedFormField label="Unit">
            <input
              v-model="form.unit"
              class="w-full p-2 border border-border rounded-md bg-surface"
              placeholder="e.g. kg, units"
            >
          </SharedFormField>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <SharedFormField label="Category">
            <select
              v-model="form.category"
              class="w-full p-2 border border-border rounded-md bg-surface"
            >
              <option value="Fruit">Fruit</option>
              <option value="Vegetable">Vegetable</option>
              <option value="Dairy">Dairy</option>
              <option value="Meat">Meat</option>
              <option value="Grains">Grains</option>
              <option value="Other">Other</option>
            </select>
          </SharedFormField>
          <SharedFormField label="Location">
            <select
              v-model="form.location"
              class="w-full p-2 border border-border rounded-md bg-surface"
            >
              <option value="Fridge">Fridge</option>
              <option value="Freezer">Freezer</option>
              <option value="Pantry">Pantry</option>
              <option value="Other">Other</option>
            </select>
          </SharedFormField>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <SharedFormField label="Expiry Date">
            <input
              v-model="form.expiresAt"
              type="date"
              class="w-full p-2 border border-border rounded-md bg-surface"
            >
          </SharedFormField>
          <SharedFormField label="Opened">
            <input
              v-model="form.isOpened"
              type="checkbox"
              class="w-4 h-4 p-2 border border-border rounded-md bg-surface"
            >
          </SharedFormField>
        </div>

        <SharedFormField label="Notes">
          <textarea
            v-model="form.notes"
            class="w-full p-2 border border-border rounded-md bg-surface"
            placeholder="Additional notes"
            rows="3"
          />
        </SharedFormField>
      </form>

      <template #footer>
        <SharedButton variant="secondary" @click="closeAddItemModal">Cancel</SharedButton>
        <SharedButton variant="primary" :disabled="submitting" @click="handleAddItem">
          {{ submitting ? "Adding…" : "Add Item" }}
        </SharedButton>
      </template>
    </SharedModal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { formatDate } from "@/utils/date";
import { useShoppingList } from "@/composables/useShoppingList";

const { items, loading, error, isStale, loadInventory, addInventoryItem } = useInventory();
const { refresh: refreshShoppingList } = useShoppingList();

// Modal state
const showAddItemModal = ref(false);
const form = reactive({
  name: "",
  category: "Other",
  location: "Other",
  quantity: 1,
  unit: "",
  expiresAt: "",
  isOpened: false,
  notes: "",
});
const errors = reactive({ name: "" });
const submitting = ref(false);

// Open add item modal
const openAddItemModal = () => {
  showAddItemModal.value = true;
  form.name = "";
  form.category = "Other";
  form.location = "Other";
  form.quantity = 1;
  form.unit = "";
  form.expiresAt = "";
  form.isOpened = false;
  form.notes = "";
  errors.name = "";
};

// Close add item modal
const closeAddItemModal = () => {
  showAddItemModal.value = false;
};

// Handle add item form submission
const handleAddItem = async () => {
  if (!form.name.trim()) {
    errors.name = "Item name is required";
    return;
  }
  errors.name = "";

  submitting.value = true;
  try {
    await addInventoryItem({
      name: form.name,
      category: form.category,
      location: form.location,
      quantity: form.quantity,
      unit: form.unit,
      expiresAt: form.expiresAt || undefined,
      isOpened: form.isOpened,
      notes: form.notes,
    });
    closeAddItemModal();
    // Refresh the shopping list as well since we added items
    await refreshShoppingList();
  } catch {
    // Error state is already handled by the composable
  } finally {
    submitting.value = false;
  }
};

// Retry loading
const retryLoad = async () => {
  try {
    await loadInventory();
  } catch {
    // The error state is already handled by the composable
  }
};

// Load inventory on component mount
try {
  await loadInventory();
} catch {
  // error state is handled by the composable
}

// Start polling for updates
onMounted(() => {
  // Initialize the polling when component is mounted
  const { startPolling } = useInventory();
  startPolling();
});

onUnmounted(() => {
  // Stop polling when component is unmounted
  const { stopPolling } = useInventory();
  stopPolling();
});
</script>
