<template>
  <div class="min-h-screen bg-bg">
    <header class="bg-surface border-b border-border px-6 py-4">
      <nav class="flex gap-6">
        <SharedNavLink name="Home" url="/" />
        <SharedNavLink name="Inventory" url="/inventory" />
        <SharedNavLink name="Recipes" url="/recipes" />
        <SharedNavLink name="Shopping" url="/shopping" />
        <SharedDevModeGuard>
          <SharedNavLink name="Development" url="/development" />
        </SharedDevModeGuard>
      </nav>
    </header>
    <main><slot /></main>
    <SharedToastContainer />
  </div>
</template>
