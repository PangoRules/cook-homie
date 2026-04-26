<template>
  <div class="inventory-page">
    <h1>Inventory</h1>
    <button @click="loadInventory">Refresh</button>
    <div v-if="loading">Loading inventory...</div>
    <div v-else-if="error">Error: {{ error }}</div>
    <div v-else-if="items.length === 0">No items</div>
    <ul v-else>
      <li v-for="item in items" :key="item.id">
        {{ item.name }} - {{ item.quantity }} {{ item.unit }}
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
const { items, loading, error, loadInventory } = useInventory();

try {
  await loadInventory();
} catch {
  // error state is handled by the composable
}
</script>
