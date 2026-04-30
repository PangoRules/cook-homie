<template>
  <div class="min-h-screen bg-bg">
    <header class="bg-surface border-b border-border px-6 py-4">
      <nav class="flex gap-6">
        <NuxtLink to="/" class="text-sm font-medium text-text-secondary hover:text-text-primary transition-colors">Home</NuxtLink>
        <NuxtLink to="/inventory" class="text-sm font-medium text-text-secondary hover:text-text-primary transition-colors">Inventory</NuxtLink>
        <NuxtLink to="/recipes" class="text-sm font-medium text-text-secondary hover:text-text-primary transition-colors">Recipes</NuxtLink>
        <NuxtLink to="/shopping" class="text-sm font-medium text-text-secondary hover:text-text-primary transition-colors">Shopping</NuxtLink>
        <SharedDevModeGuard>
          <NuxtLink to="/development" class="text-sm font-medium text-text-secondary hover:text-text-primary transition-colors">Development</NuxtLink>
        </SharedDevModeGuard>
      </nav>
    </header>
    <main><slot /></main>
    <SharedToastContainer />
  </div>
</template>
