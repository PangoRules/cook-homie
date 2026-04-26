<template>
  <div>
    <h1 class="text-2xl font-bold mb-4">Inventory</h1>
    <div v-if="loading">Loading inventory...</div>
    <div v-else-if="error" class="text-red-500">Error: {{ error }}</div>
    <div v-else>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div 
          v-for="item in inventory" 
          :key="item.id"
          class="bg-white p-4 rounded shadow"
        >
          <h2 class="text-xl font-semibold">{{ item.name }}</h2>
          <p>Quantity: {{ item.quantity }}</p>
          <p>Expiry Date: {{ item.expiryDate }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useInventory } from '@/composables/useInventory'

const { inventory, loading, error } = useInventory()
</script>