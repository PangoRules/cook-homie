<template>
  <div class="inventory-page">
    <h1>Inventory</h1>
    <button @click="openModal" class="btn-primary">New Item</button>
    <button @click="loadInventory">Refresh</button>
    <div v-if="loading">Loading inventory...</div>
    <div v-else-if="error">Error: {{ error }}</div>
    <div v-else-if="items.length === 0">No items</div>
    <ul v-else>
      <li v-for="item in items" :key="item.id">
        {{ item.name }} - {{ item.quantity }} {{ item.unit }}
      </li>
    </ul>
    
    <AddItemModal 
      :is-open="isModalOpen" 
      @close="closeModal"
      @item-added="handleItemAdded"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import AddItemModal from '~/components/Inventory/AddItemModal.vue';

const { items, loading, error, loadInventory } = useInventory();
const isModalOpen = ref(false);

try {
  await loadInventory();
} catch {
  // error state is handled by the composable
}

const openModal = () => {
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};

const handleItemAdded = () => {
  // Refresh the inventory list after adding an item
  loadInventory();
};
</script>

<style scoped>
.btn-primary {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  margin-bottom: 10px;
}
</style>
