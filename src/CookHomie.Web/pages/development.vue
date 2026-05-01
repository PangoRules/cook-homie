<template>
  <div class="p-6">
    <h1>Development</h1>

    <div class="flex border-b border-border mb-5">
      <button
        v-for="tab in ['inventory', 'polling', 'recipes']"
        :key="tab"
        :class="[
          'px-5 py-2 cursor-pointer border-none bg-transparent capitalize text-sm font-medium transition-colors border-b-2 -mb-px',
          activeTab === tab
            ? 'border-b-accent text-accent font-semibold'
            : 'border-b-transparent text-text-secondary hover:text-text-primary'
        ]"
        @click="activeTab = tab"
      >
        {{ tab }}
      </button>
    </div>

    <!-- Inventory tab -->
    <div v-if="activeTab === 'inventory'">
      <div class="flex gap-2 mb-5 flex-wrap">
        <SharedButton :disabled="seeding" @click="seedInventory">Seed Inventory</SharedButton>
        <SharedButton variant="secondary" @click="refreshInventory">Refresh</SharedButton>
        <SharedButton variant="secondary" :disabled="inventoryPolling" @click="startInventoryPolling">Start Polling</SharedButton>
        <SharedButton variant="secondary" :disabled="!inventoryPolling" @click="stopInventoryPolling">Stop Polling</SharedButton>
      </div>

      <form class="flex flex-col gap-3 mb-5 max-w-sm" @submit.prevent="addItem">
        <input v-model="newItem.name" class="input" placeholder="Item name" required />
        <input v-model="newItem.quantity" type="number" class="input" placeholder="Quantity" required />
        <select v-model="newItem.unit" class="input" required>
          <option value="">Select unit</option>
          <option v-for="u in ['pieces','grams','kilograms','milliliters','liters','packages','boxes','cans']" :key="u" :value="u">{{ u }}</option>
        </select>
        <input v-model="newItem.category" class="input" placeholder="Category" />
        <input v-model="newItem.expiresAt" type="date" class="input" placeholder="Expiry date" />
        <SharedButton type="submit">Add Item</SharedButton>
      </form>

      <div v-if="loading" class="flex flex-col gap-2">
        <SharedSkeletonBlock v-for="i in 5" :key="i" class="h-10" />
      </div>
      <SharedErrorBanner v-else-if="error" :message="error" />
      <p v-else-if="items.length === 0" class="text-text-muted text-sm">No items in inventory</p>
      <div v-else>
        <SharedStaleIndicator v-if="isStale" />
        <ul class="list-none p-0 flex flex-col gap-1 mt-3">
          <li v-for="item in items" :key="item.id" class="px-3 py-2 border border-border rounded-md text-sm">
            {{ item.name }} - {{ item.quantity }} {{ item.unit }}
            <span v-if="item.category" class="text-text-muted">({{ item.category }})</span>
            <span v-if="item.expiresAt" class="text-text-muted"> - Expires: {{ formatDate(item.expiresAt) }}</span>
          </li>
        </ul>
      </div>
    </div>

    <!-- Polling tab -->
    <div v-if="activeTab === 'polling'" class="max-w-2xl">
      <div class="mb-6">
        <h2 class="text-lg font-semibold mb-3">Polling Status</h2>
        <div class="flex items-center gap-4 mb-4">
          <span
            :class="[
              'px-3 py-1 rounded text-sm font-semibold',
              pollingActive ? 'bg-success-subtle text-success' : 'bg-error-subtle text-error'
            ]"
          >
            {{ pollingActive ? "Active" : "Inactive" }}
          </span>
          <p class="text-sm text-text-secondary m-0">Polling interval: {{ pollingInterval }}ms</p>
        </div>
        <div class="flex gap-2">
          <SharedButton :disabled="pollingActive" @click="startPollingData">Start Polling</SharedButton>
          <SharedButton variant="secondary" :disabled="!pollingActive" @click="stopPollingData">Stop Polling</SharedButton>
        </div>
      </div>

      <div>
        <h3 class="text-base font-semibold mb-3">Simulated Polling Data</h3>
        <div v-if="pollingLoading" class="flex flex-col gap-2">
          <SharedSkeletonBlock v-for="i in 3" :key="i" class="h-10" />
        </div>
        <SharedErrorBanner v-else-if="pollingError" :message="pollingError" :show-retry="true" @retry="refreshPollingData" />
        <div v-else-if="pollingData" class="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-4 mb-5">
          <div v-for="[label, value] in [['Timestamp', formatTimestamp(pollingData.timestamp)], ['Items Count', pollingData.itemsCount], ['Last polled', formatTimestamp(pollingData.lastPolled)]]" :key="label" class="flex flex-col p-3 bg-surface-hover rounded-md">
            <span class="text-xs font-semibold text-text-secondary mb-1">{{ label }}</span>
            <span class="text-lg text-accent">{{ value }}</span>
          </div>
        </div>

        <div class="mt-5 p-4 bg-surface-hover rounded-md max-h-52 overflow-y-auto">
          <h4 class="text-sm font-semibold mb-3">Recent Polling Events</h4>
          <div v-for="(log, index) in pollingLogs.slice().reverse()" :key="index" class="flex justify-between py-1 border-b border-border text-sm last:border-0">
            <span class="text-text-muted text-xs">{{ formatTimestamp(log.timestamp) }}</span>
            <span class="text-text-primary">{{ log.message }}</span>
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

const { items, loading, error, isStale, loadInventory, startPolling, stopPolling, refresh, addInventoryItem } = useInventory();
const { pushSuccess, pushError } = useToast();

const seeding = ref(false);
const inventoryPolling = ref(false);
const newItem = ref({ name: "", quantity: 1, unit: "", category: "", location: "", expiresAt: "" });

const pollingActive = ref(false);
const pollingInterval = ref(3000);
const pollingData = ref<{ timestamp: number; itemsCount: number; lastPolled: number } | null>(null);
const pollingLoading = ref(false);
const pollingError = ref<string | null>(null);
const pollingLogs = ref<{ timestamp: number; message: string }[]>([]);

const pollingFetch = usePollingFetch<{ timestamp: number; itemsCount: number; lastPolled: number }>(
  "/api/polling-data",
  {
    pollIntervalMs: pollingInterval.value,
    onSuccess: (data) => { pollingData.value = data; addPollingLog(`Fetched data: ${data.itemsCount} items`); },
    onError: (err) => { pollingError.value = err.message; addPollingLog(`Error: ${err.message}`); },
  }
);

function addPollingLog(message: string) {
  pollingLogs.value.push({ timestamp: Date.now(), message });
  if (pollingLogs.value.length > 10) pollingLogs.value.shift();
}

function formatTimestamp(timestamp: number) { return new Date(timestamp).toLocaleTimeString(); }
function formatDate(dateString: string | null) { return dateString ? new Date(dateString).toLocaleDateString() : ""; }

try { await loadInventory(); } catch { /* handled by composable */ }

async function seedInventory() {
  seeding.value = true;
  try { await $fetch("/api/dev/inventory-seed", { method: "POST" }); await loadInventory(); pushSuccess("Inventory seeded successfully"); }
  catch (err) { pushError("Failed to seed inventory"); console.error(err); }
  finally { seeding.value = false; }
}

async function refreshInventory() {
  try { await refresh(); pushSuccess("Inventory refreshed"); }
  catch (err) { pushError("Failed to refresh inventory"); console.error(err); }
}

async function addItem() {
  try {
    await addInventoryItem({ name: newItem.value.name, quantity: newItem.value.quantity, unit: newItem.value.unit, category: newItem.value.category, location: newItem.value.location, expiresAt: newItem.value.expiresAt, isOpened: false });
    newItem.value = { name: "", quantity: 1, unit: "", category: "", location: "", expiresAt: "" };
    pushSuccess("Item added successfully");
  } catch (err) { pushError("Failed to add item"); console.error(err); }
}

function startInventoryPolling() { inventoryPolling.value = true; startPolling(); }
function stopInventoryPolling() { inventoryPolling.value = false; stopPolling(); }

async function startPollingData() {
  pollingActive.value = true; pollingError.value = null;
  try { await pollingFetch.start(); addPollingLog("Polling started"); }
  catch (err) { pollingError.value = err instanceof Error ? err.message : "Failed to start polling"; addPollingLog(`Error starting polling: ${err}`); }
}

function stopPollingData() { pollingActive.value = false; pollingFetch.stop(); addPollingLog("Polling stopped"); }
function refreshPollingData() { pollingFetch.refresh().then(() => addPollingLog("Manual refresh performed")); }
</script>
