<template>
  <article class="recipe-card" @click="handleClick">
    <div class="recipe-card__body">
      <div class="recipe-card__tags">
        <span v-for="tag in recipe.tags" :key="tag" class="tag">{{ tag }}</span>
      </div>
      <h3 class="recipe-card__name">{{ recipe.name }}</h3>
      <p class="recipe-card__meta">
        {{ recipe.prepMinutes + recipe.cookMinutes }} min · {{ recipe.instructions.slice(0, 60) }}...
      </p>
    </div>
  </article>
</template>

<script setup lang="ts">
import type { Recipe } from "~/types";
const props = defineProps<{ recipe: Recipe }>();
const emit = defineEmits(["click"]);
const handleClick = () => {
  emit("click", props.recipe.id);
};
</script>

<style scoped>
.recipe-card {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: var(--space-5);
  cursor: pointer;
  transition: all var(--transition-fast);
  box-shadow: var(--shadow-sm);
}
.recipe-card:hover { box-shadow: var(--shadow-md); transform: translateY(-2px); }

.recipe-card__tags { display: flex; gap: var(--space-2); flex-wrap: wrap; margin-bottom: var(--space-3); }

.tag {
  background: var(--color-accent-subtle);
  color: var(--color-accent);
  border-radius: var(--radius-full);
  padding: var(--space-1) var(--space-3);
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.recipe-card__name {
  font-family: var(--font-display);
  font-size: 18px;
  font-weight: 600;
  color: var(--color-text-primary);
  margin: 0 0 var(--space-2);
}

.recipe-card__meta {
  font-size: 13px;
  color: var(--color-text-muted);
  margin: 0;
  line-height: 1.4;
}
</style>