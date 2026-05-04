<template>
  <article
    class="bg-surface border border-border rounded-lg p-5 cursor-pointer transition-all shadow-sm hover:shadow-md hover:-translate-y-0.5"
  >
    <NuxtLink :to="`recipes/${recipe.id}`">
      <div class="flex gap-2 flex-wrap mb-3">
        <span
          v-for="tag in recipe.tags"
          :key="tag"
          class="bg-accent-subtle text-accent rounded-full px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.05em]"
        >
          {{ tag }}
        </span>
      </div>
      <h3 class="font-display text-lg font-semibold text-text-primary mb-2">{{ recipe.name }}</h3>
      <p class="text-[13px] text-text-muted leading-snug m-0">
        {{ recipe.prepMinutes + recipe.cookMinutes }} min ·
        {{ recipe.instructions.slice(0, 60) }}...
      </p>
    </NuxtLink>
  </article>
</template>

<script setup lang="ts">
  import type { Recipe } from "~/types";
  defineProps<{ recipe: Recipe }>();
</script>
