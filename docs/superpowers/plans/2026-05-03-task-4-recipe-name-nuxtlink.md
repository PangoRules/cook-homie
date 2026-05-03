# Task 4: Recipe Name as Accessible `NuxtLink` on Dashboard
**Branch:** `task/task-4-recipe-name-nuxtlink`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Modify: `src/CookHomie.Web/pages/index.vue:83`

## Steps

- [ ] **Step 1: Convert `<span>` to `<NuxtLink>` in dashboard recipe ideas panel**

In `src/CookHomie.Web/pages/index.vue`, on line 83, replace:

```vue
<span class="text-text-primary font-medium">{{ idea.name }}</span>
```

With:

```vue
<NuxtLink
  :to="`/recipes/${idea.id}`"
  class="text-text-primary font-medium text-inherit hover:underline focus:outline-none focus:ring-2 focus:ring-accent rounded"
>
  {{ idea.name }}
</NuxtLink>
```

Add `text-inherit` to make link color inherit parent (stays visually neutral). Add `focus:ring` for accessibility. The row structure and other actions remain intact.

- [ ] **Step 2: Verify existing hover styles don't conflict**

No changes needed to CSS — the link inherits row styles. The `focus:ring` ensures keyboard accessibility.

- [ ] **Step 3: Commit**

```bash
git add src/CookHomie.Web/pages/index.vue
git commit -m "feat(web): make dashboard recipe name an accessible NuxtLink"
```
