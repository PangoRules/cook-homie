import { ref } from 'vue';
import type { InventoryItem, AddInventoryItemPayload } from '../../types';

export const useAddItemForm = () => {
  const isSubmitting = ref(false);
  const error = ref<string | null>(null);
  const success = ref(false);

  const addInventoryItem = async (payload: AddInventoryItemPayload): Promise<InventoryItem> => {
    isSubmitting.value = true;
    error.value = null;
    success.value = false;

    try {
      // This would normally call the API
      // const result = await $fetch<InventoryItem>("/api/inventory", {
      //   method: "POST",
      //   body: payload
      // });
      
      // For now, simulate a successful API call
      console.log("Would add item:", payload);
      
      // Simulating API delay
      await new Promise(resolve => setTimeout(resolve, 500));
      
      // Simulate returning an item with a generated ID
      const newItem: InventoryItem = {
        id: Math.random().toString(36).substring(2, 9),
        ...payload,
        category: '',
        location: '',
        isOpened: false,
        notes: ''
      };
      
      success.value = true;
      return newItem;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to add inventory item";
      throw err;
    } finally {
      isSubmitting.value = false;
    }
  };

  return {
    isSubmitting,
    error,
    success,
    addInventoryItem
  };
};