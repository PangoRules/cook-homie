<template>
  <main>
    <h1>CookHomie Spike Web</h1>
    <p v-if="data?.message">Message from API: {{ data.message }}</p>
    <p v-else-if="error">Unable to load spike message.</p>
    <p v-else>Loading spike message...</p>
  </main>
</template>

<script setup lang="ts">
const { data, error } = useFetch<{ message: string }>("/api/spike/hello");
</script>
