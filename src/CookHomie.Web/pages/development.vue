<template>
  <div class="development-page">
    <h1>Development</h1>

    <div class="tabs">
      <button :class="{ active: activeTab === 'inventory' }" @click="activeTab = 'inventory'">
        Inventory
      </button>
      <button :class="{ active: activeTab === 'polling' }" @click="activeTab = 'polling'">
        Polling
      </button>
      <button :class="{ active: activeTab === 'recipes' }" @click="activeTab = 'recipes'">
        Recipes
      </button>
    </div>

    <div class="tab-content">
      <div v-if="activeTab === 'inventory'" class="inventory-tab">...</div>

      <div v-if="activeTab === 'polling'" class="polling-tab">...</div>

      <div v-if="activeTab === 'recipes'" class="recipes-tab">...</div>
    </div>

    <div class="tab-content">
      <div v-if="activeTab === 'inventory'" class="inventory-tab">
        <div class="inventory-header">
          <button :disabled="seeding" @click="seedInventory">Seed Inventory</button>
          <button @click="refreshInventory">Refresh</button>
          <button :disabled="inventoryPolling" @click="startInventoryPolling">Start Polling</button>
          <button :disabled="!inventoryPolling" @click="stopInventoryPolling">Stop Polling</button>
        </div>

        <form class="add-item-form" @submit.prevent="addItem">
          <input v-model="newItem.name" placeholder="Item name" required />
          <input v-model="newItem.quantity" type="number" placeholder="Quantity" required />
          <select v-model="newItem.unit" required>
            <option value="">Select unit</option>
            <option value="pieces">pieces</option>
            <option value="grams">grams</option>
            <option value="kilograms">kilograms</option>
            <option value="milliliters">milliliters</option>
            <option value="liters">liters</option>
            <option value="packages">packages</option>
            <option value="boxes">boxes</option>
            <option value="cans">cans</option>
          </select>
          <input v-model="newItem.category" placeholder="Category" />
          <input v-model="newItem.expiresAt" type="date" placeholder="Expiry date" />
          <button type="submit">Add Item</button>
        </form>

        <div v-if="loading">
          <SkeletonBlock v-for="i in 5" :key="i" />
        </div>

        <ErrorBanner v-else-if="error" :message="error" />

        <div v-else-if="items.length === 0">No items in inventory</div>

        <div v-else>
          <StaleIndicator v-if="isStale" />
          <ul class="inventory-list">
            <li v-for="item in items" :key="item.id" class="inventory-item">
              {{ item.name }} - {{ item.quantity }} {{ item.unit }}
              <span v-if="item.category">({{ item.category }})</span>
              <span v-if="item.expiresAt"> - Expires: {{ formatDate(item.expiresAt) }}</span>
            </li>
          </ul>
        </div>
      </div>

      <div v-if="activeTab === 'polling'" class="polling-tab">
        <div class="polling-header">
          <h2>Polling Status</h2>
          <div class="polling-status">
            <div
              class="status-indicator"
              :class="{ 'status-active': pollingActive, 'status-inactive': !pollingActive }"
            >
              {{ pollingActive ? "Active" : "Inactive" }}
            </div>
            <p>Polling interval: {{ pollingInterval }}ms</p>
          </div>

          <div class="polling-controls">
            <button :disabled="pollingActive" class="btn-primary" @click="startPollingData">
              Start Polling
            </button>
            <button :disabled="!pollingActive" class="btn-secondary" @click="stopPollingData">
              Stop Polling
            </button>
          </div>
        </div>

        <div class="polling-data">
          <h3>Simulated Polling Data</h3>

          <div v-if="pollingLoading">
            <SkeletonBlock v-for="i in 3" :key="i" />
          </div>

          <ErrorBanner
            v-else-if="pollingError"
            :message="pollingError"
            :show-retry="true"
            @retry="refreshPollingData"
          />

          <div v-else-if="pollingData">
            <div class="polling-data-grid">
              <div class="data-item">
                <span class="data-label">Timestamp:</span>
                <span class="data-value">{{ formatTimestamp(pollingData.timestamp) }}</span>
              </div>
              <div class="data-item">
                <span class="data-label">Items Count:</span>
                <span class="data-value">{{ pollingData.itemsCount }}</span>
              </div>
              <div class="data-item">
                <span class="data-label">Last polled:</span>
                <span class="data-value">{{ formatTimestamp(pollingData.lastPolled) }}</span>
              </div>
            </div>
          </div>

          <div class="polling-log">
            <h4>Recent Polling Events</h4>
            <div
              v-for="(log, index) in pollingLogs.slice().reverse()"
              :key="index"
              class="log-item"
            >
              <span class="log-timestamp">{{ formatTimestamp(log.timestamp) }}</span>
              <span class="log-message">{{ log.message }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useInventory } from "@/composables/useInventory";
import { useToast } from "@/composables/useToast";
import { usePollingFetch } from "@/composables/usePollingFetch";

const activeTab = ref("inventory");

const {
  items,
  loading,
  error,
  isStale,
  loadInventory,
  startPolling,
  stopPolling,
  refresh,
  addInventoryItem,
} = useInventory();
const { pushSuccess, pushError } = useToast();

const seeding = ref(false);
const inventoryPolling = ref(false);
const newItem = ref({
  name: "",
  quantity: 1,
  unit: "",
  category: "",
  location: "",
  expiresAt: "",
});

// Polling tab specific logic
const pollingActive = ref(false);
const pollingInterval = ref(3000); // 3 seconds for demo purposes
const pollingData = ref<{ timestamp: number; itemsCount: number; lastPolled: number } | null>(null);
const pollingLoading = ref(false);
const pollingError = ref<string | null>(null);
const pollingLogs = ref<{ timestamp: number; message: string }[]>([]);

const pollingFetch = usePollingFetch<{ timestamp: number; itemsCount: number; lastPolled: number }>(
  "/api/polling-data",
  {
    pollIntervalMs: pollingInterval.value,
    onSuccess: (data) => {
      pollingData.value = data;
      addPollingLog(`Fetched data: ${data.itemsCount} items`);
    },
    onError: (err) => {
      pollingError.value = err.message;
      addPollingLog(`Error: ${err.message}`);
    },
  }
);

function addPollingLog(message: string) {
  pollingLogs.value.push({
    timestamp: Date.now(),
    message,
  });

  // Keep only last 10 logs
  if (pollingLogs.value.length > 10) {
    pollingLogs.value.shift();
  }
}

function formatTimestamp(timestamp: number) {
  return new Date(timestamp).toLocaleTimeString();
}

try {
  await loadInventory();
} catch {
  // Error state is handled by the composable
}

async function seedInventory() {
  seeding.value = true;
  try {
    await $fetch("/api/dev/inventory-seed", { method: "POST" });
    await loadInventory();
    pushSuccess("Inventory seeded successfully");
  } catch (err) {
    pushError("Failed to seed inventory");
    console.error(err);
  } finally {
    seeding.value = false;
  }
}

async function refreshInventory() {
  try {
    await refresh();
    pushSuccess("Inventory refreshed");
  } catch (err) {
    pushError("Failed to refresh inventory");
    console.error(err);
  }
}

async function addItem() {
  try {
    await addInventoryItem({
      name: newItem.value.name,
      quantity: newItem.value.quantity,
      unit: newItem.value.unit,
      category: newItem.value.category,
      location: newItem.value.location,
      expiresAt: newItem.value.expiresAt,
      isOpened: false,
    });
    newItem.value = { name: "", quantity: 1, unit: "", category: "", location: "", expiresAt: "" };
    pushSuccess("Item added successfully");
  } catch (err) {
    pushError("Failed to add item");
    console.error(err);
  }
}

function formatDate(dateString: string | null) {
  if (!dateString) return "";
  return new Date(dateString).toLocaleDateString();
}

function startInventoryPolling() {
  inventoryPolling.value = true;
  startPolling();
}

function stopInventoryPolling() {
  inventoryPolling.value = false;
  stopPolling();
}

// Polling tab functions
async function startPollingData() {
  pollingActive.value = true;
  pollingError.value = null;

  try {
    await pollingFetch.start();
    addPollingLog("Polling started");
  } catch (err) {
    pollingError.value = err instanceof Error ? err.message : "Failed to start polling";
    addPollingLog(`Error starting polling: ${err}`);
  }
}

function stopPollingData() {
  pollingActive.value = false;
  pollingFetch.stop();
  addPollingLog("Polling stopped");
}

function refreshPollingData() {
  pollingFetch.refresh().then(() => {
    addPollingLog("Manual refresh performed");
  });
}
</script>

<style scoped>
.development-page {
  padding: 20px;
}

.tabs {
  display: flex;
  border-bottom: 1px solid #ccc;
  margin-bottom: 20px;
}

.tabs button {
  padding: 10px 20px;
  cursor: pointer;
  border: none;
  background: transparent;
  border-bottom: 3px solid transparent;
}

.tabs button.active {
  border-bottom: 3px solid #007bff;
  font-weight: bold;
}

.tab-content {
  margin-top: 20px;
}

.inventory-header {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  flex-wrap: wrap;
}

.add-item-form {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 20px;
  max-width: 400px;
}

.add-item-form input,
.add-item-form select {
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.add-item-form button {
  padding: 8px 16px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.add-item-form button:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.inventory-list {
  list-style: none;
  padding: 0;
}

.inventory-item {
  padding: 10px;
  border: 1px solid #eee;
  margin-bottom: 5px;
  border-radius: 4px;
}

.polling-tab {
  padding: 20px;
}

.polling-header {
  margin-bottom: 30px;
}

.polling-status {
  margin: 15px 0;
  display: flex;
  align-items: center;
  gap: 15px;
}

.status-indicator {
  padding: 8px 12px;
  border-radius: 4px;
  font-weight: bold;
}

.status-active {
  background: #d4edda;
  color: #155724;
}

.status-inactive {
  background: #f8d7da;
  color: #721c24;
}

.polling-controls {
  margin: 15px 0;
}

.btn-primary {
  padding: 8px 16px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.btn-primary:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.btn-secondary {
  padding: 8px 16px;
  background: #6c757d;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  margin-left: 10px;
}

.btn-secondary:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.polling-data-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 15px;
  margin: 20px 0;
}

.data-item {
  display: flex;
  flex-direction: column;
  padding: 10px;
  background: #f8f9fa;
  border-radius: 4px;
}

.data-label {
  font-weight: bold;
  margin-bottom: 5px;
  color: #495057;
}

.data-value {
  font-size: 18px;
  color: #007bff;
}

.polling-log {
  margin-top: 20px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 4px;
  max-height: 200px;
  overflow-y: auto;
}

.log-item {
  display: flex;
  justify-content: space-between;
  padding: 5px 0;
  border-bottom: 1px solid #dee2e6;
}

.log-timestamp {
  color: #6c757d;
  font-size: 12px;
}

.log-message {
  color: #343a40;
  font-size: 14px;
}
</style>
