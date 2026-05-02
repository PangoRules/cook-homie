<template>
  <SharedModal ref="modalRef" title="Add Recipe" @close="$emit('close')">
    <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
      <SharedFormField label="Name" :error="errors.name" required>
        <input v-model="form.name" class="input" placeholder="e.g. Banana Bread" >
      </SharedFormField>

      <div class="grid grid-cols-2 gap-4">
        <SharedFormField label="Prep (min)">
          <input v-model.number="form.prepMinutes" type="number" min="0" class="input" >
        </SharedFormField>
        <SharedFormField label="Cook (min)">
          <input v-model.number="form.cookMinutes" type="number" min="0" class="input" >
        </SharedFormField>
      </div>

      <SharedFormField label="Instructions" :error="errors.instructions" required>
        <textarea v-model="form.instructions" class="input" rows="4" placeholder="Step by step..." />
      </SharedFormField>

      <SharedFormField label="Tags">
        <SharedTagInput v-model="form.tags" placeholder="Type tag, press Enter" />
      </SharedFormField>
    </form>

    <template #footer>
      <SharedButton variant="secondary" @click="onCancel">Cancel</SharedButton>
      <SharedButton variant="primary" :disabled="submitting" @click="handleSubmit">
        {{ submitting ? "Adding…" : "Add Recipe" }}
      </SharedButton>
    </template>
  </SharedModal>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { useToast } from "@/composables/useToast";

const emit = defineEmits(["added", "close"]);
const modalRef = ref();

const form = reactive({
  name: "",
  prepMinutes: 0,
  cookMinutes: 0,
  instructions: "",
  tags: [] as string[],
});

const errors = reactive({ name: "", instructions: "" });
const submitting = ref(false);

const validate = () => {
  let isValid = true;
  if (!form.name.trim()) { errors.name = "Recipe name is required"; isValid = false; }
  else errors.name = "";
  if (!form.instructions.trim()) { errors.instructions = "Instructions are required"; isValid = false; }
  else errors.instructions = "";
  return isValid;
};

const resetForm = () => {
  form.name = ""; form.prepMinutes = 0; form.cookMinutes = 0; form.instructions = "";
  form.tags = []; errors.name = ""; errors.instructions = "";
};

const onCancel = () => {
  resetForm();
  modalRef.value.startClose();
};

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    const toast = useToast();
    const response = await $fetch("/api/recipes", {
      method: "POST",
      body: { ...form },
    });
    emit("added", response);
    resetForm();
    toast.pushSuccess("Recipe added successfully!");
    modalRef.value.startClose();
  } catch {
    useToast().pushError("Failed to add recipe");
  } finally {
    submitting.value = false;
  }
};
</script>
