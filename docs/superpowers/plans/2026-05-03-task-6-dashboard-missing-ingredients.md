# Task 6: Dashboard Missing Ingredient Expansion + Shopping Actions
**Branch:** `task/task-6-dashboard-missing-ingredients`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Modify: `src/CookHomie.Web/pages/index.vue`
- Modify: `src/CookHomie.Web/composables/useShoppingList.ts` (add `addBulk` if not present, check after Task 9)
- Modify: `src/CookHomie.Web/server/api/recipes/[id]/missing.get.ts`

## Dependencies
- Task 9 (shopping API) must be completed first — this task calls `useShoppingList().addBulk()`.

## Steps

- [ ] **Step 1: Confirm `useShoppingList` exposes `addBulk`**

After Task 9, verify `useShoppingList.ts` exposes `addBulk(names: string[])` that calls `POST /api/shopping/bulk`. If not, add it in this task.

- [ ] **Step 2: Add expand/collapse state per recipe idea in dashboard**

In `src/CookHomie.Web/pages/index.vue`, add per-idea expansion tracking:

```typescript
const expandedIdeaId = ref<string | null>(null);
const loadingMissing = ref(false);
const missingCache = ref<Record<string, string[]>>({});

const toggleExpand = async (idea: DashboardRecipeIdea) => {
  if (expandedIdeaId.value === idea.id) {
    expandedIdeaId.value = null;
    return;
  }
  expandedIdeaId.value = idea.id;
  if (!missingCache.value[idea.id]) {
    loadingMissing.value = true;
    try {
      const missing = await $fetch<string[]>(`/api/recipes/${idea.id}/missing`);
      missingCache.value[idea.id] = missing;
    } finally {
      loadingMissing.value = false;
    }
  }
};
```

- [ ] **Step 3: Add expand UI to recipe ideas panel**

In the recipe ideas loop, wrap the row + add an expandable section:

```vue
<div v-for="idea in data?.recipeIdeas" :key="idea.id" class="border-b border-border">
  <div class="flex items-center justify-between py-2 cursor-pointer" @click="toggleExpand(idea)">
    <div>
      <NuxtLink :to="`/recipes/${idea.id}`" class="text-text-primary font-medium hover:underline">
        {{ idea.name }}
      </NuxtLink>
      <span class="block text-text-muted text-sm">{{ idea.matchedCount }} matched, {{ idea.missingCount }} missing</span>
    </div>
    <span v-if="idea.missingCount > 0">{{ expandedIdeaId === idea.id ? "▲" : "▼" }}</span>
  </div>

  <!-- Expanded: show missing ingredients -->
  <div v-if="expandedIdeaId === idea.id" class="pl-4 pb-3">
    <div v-if="loadingMissing" class="text-text-muted text-sm">Loading…</div>
    <div v-else-if="missingCache[idea.id]?.length > 0">
      <ul class="list-none p-0 m-0 flex flex-col gap-1">
        <li v-for="ing in missingCache[idea.id]" :key="ing" class="flex items-center justify-between text-sm">
          <span>{{ ing }}</span>
          <SharedButton size="small" variant="secondary" @click.stop="addMissing(idea.id, ing)">
            + Add
          </SharedButton>
        </li>
      </ul>
      <SharedButton class="mt-2" variant="primary" size="small" @click.stop="addAllMissing(idea.id)">
        Add all missing
      </SharedButton>
    </div>
    <div v-else class="text-text-muted text-sm">No missing ingredients!</div>
  </div>
</div>
```

- [ ] **Step 4: Implement `addMissing` and `addAllMissing`**

```typescript
const addMissing = async (recipeId: string, ingredientName: string) => {
  const { addBulk } = useShoppingList();
  try {
    await addBulk([ingredientName]);
    useToast().pushSuccess(`"${ingredientName}" added to shopping list`);
  } catch {
    useToast().pushError("Already on shopping list");
  }
};

const addAllMissing = async (recipeId: string) => {
  const { addBulk } = useShoppingList();
  const missing = missingCache.value[recipeId] ?? [];
  try {
    await addBulk(missing);
    useToast().pushSuccess(`${missing.length} items added to shopping list`);
  } catch {
    useToast().pushError("Some items already on list");
  }
};
```

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/pages/index.vue
git commit -m "feat(web): add missing ingredient expand UI and bulk add to shopping list"
```
