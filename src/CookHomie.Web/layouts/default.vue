<template>
  <div class="app-layout">
    <header class="header">
      <nav>
        <NuxtLink to="/">Home</NuxtLink>
        <NuxtLink to="/inventory">Inventory</NuxtLink>
        <NuxtLink to="/recipes">Recipes</NuxtLink>
        <NuxtLink to="/shopping">Shopping</NuxtLink>
      </nav>
    </header>
    <main class="main">
      <slot />
    </main>
    <ToastContainer />
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.header {
  padding: 1rem;
  background: #f5f5f5;
  border-bottom: 1px solid #e0e0e0;
}

.header nav {
  display: flex;
  gap: 1rem;
}

.main {
  flex: 1;
  padding: 1rem;
}
</style>
