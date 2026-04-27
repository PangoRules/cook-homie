<template>
  <main>
    <h1>CookHomie Web</h1>
    <p v-if="data?.message">Message from API: {{ data.message }}</p>
    <p v-else-if="error">Unable to load message.</p>
    <p v-else>Loading message...</p>
  </main>
</template>

<script setup lang="ts">
const { data, error } = useFetch<{ message: string }>("/api/hello");
</script>
