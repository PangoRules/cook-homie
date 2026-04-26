<template>
  <div class="recipe-detail">
    <h1>Recipe Detail</h1>
    <p>This is the recipe detail page.</p>
    <p>Recipe ID: {{ $route.params.id }}</p>
  </div>
</template>

<script setup>
//
</script>

<style scoped>
.recipe-detail {
  padding: 20px;
}
</style>