<template>
  <SharedModal
    ref="modalRef"
    :title="localInventoryItem.name"
    max-width="max-w-lg"
    @close="$emit('close')"
  >
    <pre>{{ localInventoryItem }}</pre>
    <pre>{{ item }}</pre>
    <form class="flex flex-col gap-4" @submit.prevent="handleSave">
      <SharedFormField label="Name" :error="errors.name" required>
        <input v-model="form.name" class="input" />
      </SharedFormField>

      <div class="grid grid-cols-2 gap-4">
        <SharedFormField label="Category" :error="errors.category" required>
          <input v-model="form.category" class="input" />
        </SharedFormField>
        <SharedFormField label="Location" :error="errors.location" required>
          <select v-model="form.location" class="input">
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
          <input v-model="form.unit" class="input" />
        </SharedFormField>
      </div>

      <SharedFormField label="Expires At">
        <input v-model="form.expiresAt" type="date" class="input" />
      </SharedFormField>

      <SharedFormField label="Notes">
        <textarea v-model="form.notes" class="input" rows="2" />
      </SharedFormField>
    </form>

    <template #footer>
      <SharedButton variant="secondary" @click="onCancel">Cancel</SharedButton>
      <SharedButton variant="primary" :disabled="saving" @click="handleSave">
        {{ saving ? "Saving…" : "Save Changes" }}
      </SharedButton>
    </template>

    <template v-if="mode === 'expiring'" #expiring-actions>
      <div class="flex flex-col gap-2 px-6 pb-4 border-t border-border mt-4">
        <SharedButton variant="secondary" class="w-full" @click="$emit('dismiss')">
          Dismiss
        </SharedButton>
        <SharedButton variant="secondary" class="w-full" @click="showDiscard = true">
          Discard leftover
        </SharedButton>
        <SharedButton variant="secondary" class="w-full" @click="$emit('restocked')">
          Already restocked
        </SharedButton>
        <SharedButton variant="secondary" class="w-full" @click="$emit('addToShoppingList')">
          Add to shopping list
        </SharedButton>
      </div>
    </template>
  </SharedModal>

  <Teleport to="body">
    <div
      v-if="showDiscard"
      class="fixed inset-0 bg-black/50 flex justify-center items-center z-[1001]"
    >
      <div class="bg-surface rounded-lg shadow-lg w-[90%] max-w-sm p-6">
        <h3 class="font-display text-lg font-semibold mb-4">Discard leftover</h3>
        <SharedFormField label="Amount to discard">
          <input v-model.number="discardAmount" type="number" min="0" class="input" />
        </SharedFormField>
        <div class="flex justify-end gap-2 mt-4">
          <SharedButton variant="secondary" @click="showDiscard = false">Cancel</SharedButton>
          <SharedButton variant="primary" @click="handleDiscard">Confirm</SharedButton>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
  import { ref, reactive, watch } from "vue";
  import type { InventoryItem, EditInventoryItemPayload } from "~/types";
  import { useToast } from "~/composables/useToast";

  interface Props {
    item: InventoryItem | null;
    mode?: "inventory" | "expiring";
  }

  const props = withDefaults(defineProps<Props>(), {
    mode: "inventory",
  });

  const defaultItem: InventoryItem = {
    id: "",
    name: "",
    category: "",
    location: "",
    isOpened: false,
    quantity: 0,
    unit: "",
  };
  const localInventoryItem = computed(() => props.item ?? defaultItem);
  const emit = defineEmits<{
    (e: "close" | "dismiss" | "restocked" | "addToShoppingList"): void;
    (e: "edited", payload: EditInventoryItemPayload): void;
    (e: "discard", payload: { amount: number }): void;
  }>();

  const modalRef = ref();
  const { pushSuccess, pushError } = useToast();

  const form = reactive({
    name: localInventoryItem.value.name,
    category: localInventoryItem.value.category,
    location: localInventoryItem.value.location,
    quantity: localInventoryItem.value.quantity,
    unit: localInventoryItem.value.unit,
    isOpened: localInventoryItem.value.isOpened,
    expiresAt: localInventoryItem.value.expiresAt ?? "",
    notes: localInventoryItem.value.notes ?? "",
  });

  const errors = reactive({ name: "", category: "", location: "", quantity: "" });
  const saving = ref(false);
  const showDiscard = ref(false);
  const discardAmount = ref(0);

  watch(
    () => localInventoryItem,
    (newItem) => {
      form.name = newItem.value.name;
      form.category = newItem.value.category;
      form.location = newItem.value.location;
      form.quantity = newItem.value.quantity;
      form.unit = newItem.value.unit;
      form.isOpened = newItem.value.isOpened;
      form.expiresAt = newItem.value.expiresAt ?? "";
      form.notes = newItem.value.notes ?? "";
    }
  );

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

  const onCancel = () => {
    modalRef.value.startClose();
  };

  const handleSave = async () => {
    if (!validate()) return;
    saving.value = true;
    try {
      const payload: EditInventoryItemPayload = {
        name: form.name,
        category: form.category,
        location: form.location,
        quantity: form.quantity,
        unit: form.unit,
        isOpened: form.isOpened,
        expiresAt: form.expiresAt || undefined,
        notes: form.notes || undefined,
      };
      emit("edited", payload);
      pushSuccess(`"${form.name}" updated!`);
      modalRef.value.startClose();
    } catch (err) {
      pushError(err instanceof Error ? err : new Error("Failed to update item"));
    } finally {
      saving.value = false;
    }
  };

  const handleDiscard = () => {
    emit("discard", { amount: discardAmount.value });
    showDiscard.value = false;
    modalRef.value.startClose();
  };
</script>
