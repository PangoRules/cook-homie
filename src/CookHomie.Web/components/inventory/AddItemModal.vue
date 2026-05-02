<template>
  <div>
    <form @submit.prevent="handleSubmit">
      <input v-model="form.name" name="name" >
      <button type="submit">Add</button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";

const form = ref({
  name: "",
});

const emit = defineEmits(["added"]);

const handleSubmit = async () => {
  emit("added", form.value);
};
</script>
