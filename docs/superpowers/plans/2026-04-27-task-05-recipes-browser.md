# Milestone 2 — Recipes Browser

**Branch:** `task/recipes-browser`
**Parent branch:** `feat/milestone-2-frontend`

> **For agentic workers:** Each step is `- [ ]`. Implement task-by-task on its own branch. Commit after each task.

**Goal:** Build the Recipes index page with RecipeCard grid, AddRecipeModal form, useRecipes composable, and all the wiring for 30s polling.

**Prerequisite:** Complete Tasks 1–3, 6, 7, 9 (design tokens, shared primitives, types, mock endpoints, CSS system).

---

## Task 12 — RecipeCard component

**Files:**
- Create: `src/CookHomie.Web/components/recipes/RecipeCard.vue`
- Create: `src/CookHomie.Web/tests/recipe-card.spec.ts`

- [ ] **Step 1: Write RecipeCard**

```vue
<!-- src/CookHomie.Web/components/recipes/RecipeCard.vue -->
<template>
  <article class="recipe-card" @click="$emit('click')">
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
defineProps<{ recipe: Recipe }>();
defineEmits(["click"]);
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
```

- [ ] **Step 2: Write tests**

```typescript
// src/CookHomie.Web/tests/recipe-card.spec.ts
import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import RecipeCard from "../components/recipes/RecipeCard.vue";

describe("RecipeCard", () => {
  const recipe = {
    id: "r1",
    name: "Pancakes",
    instructions: "Mix and cook.",
    prepMinutes: 5,
    cookMinutes: 10,
    tags: ["breakfast"]
  };

  it("renders recipe name", () => {
    const wrapper = mount(RecipeCard, { props: { recipe } });
    expect(wrapper.find(".recipe-card__name").text()).toBe("Pancakes");
  });

  it("renders tags", () => {
    const wrapper = mount(RecipeCard, { props: { recipe } });
    expect(wrapper.find(".tag").text()).toBe("breakfast");
  });

  it("emits click with recipe id", async () => {
    const wrapper = mount(RecipeCard, { props: { recipe } });
    await wrapper.trigger("click");
    expect(wrapper.emitted("click")?.[0]).toEqual([recipe.id]);
  });
});
```

- [ ] **Step 3: Run tests**

Run: `cd src/CookHomie.Web && npx vitest run tests/recipe-card.spec.ts`
Expected: PASS

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/components/recipes/RecipeCard.vue src/CookHomie.Web/tests/recipe-card.spec.ts
git commit -m "feat(web): add RecipeCard component"
```

---

## Task 13 — AddRecipeModal component

**Files:**
- Create: `src/CookHomie.Web/components/recipes/AddRecipeModal.vue`
- Create: `src/CookHomie.Web/tests/add-recipe-modal.spec.ts`

- [ ] **Step 1: Write AddRecipeModal**

```vue
<!-- src/CookHomie.Web/components/recipes/AddRecipeModal.vue -->
<template>
  <div class="modal-backdrop" @click.self="$emit('close')">
    <div class="modal">
      <header class="modal__header">
        <h2 class="modal__title">Add Recipe</h2>
        <button class="modal__close" @click="$emit('close')">×</button>
      </header>

      <form class="modal__form" @submit.prevent="handleSubmit">
        <div class="field">
          <label class="field__label">Name *</label>
          <input v-model="form.name" class="field__input" placeholder="e.g. Banana Bread" required />
          <span v-if="errors.name" class="field__error">{{ errors.name }}</span>
        </div>

        <div class="field-row">
          <div class="field">
            <label class="field__label">Prep (min)</label>
            <input v-model.number="form.prepMinutes" type="number" min="0" class="field__input" />
          </div>
          <div class="field">
            <label class="field__label">Cook (min)</label>
            <input v-model.number="form.cookMinutes" type="number" min="0" class="field__input" />
          </div>
        </div>

        <div class="field">
          <label class="field__label">Instructions *</label>
          <textarea v-model="form.instructions" class="field__input" rows="4" placeholder="Step by step..." required />
          <span v-if="errors.instructions" class="field__error">{{ errors.instructions }}</span>
        </div>

        <div class="field">
          <label class="field__label">Tags (comma separated)</label>
          <input v-model="tagsInput" class="field__input" placeholder="quick, vegetarian" />
        </div>

        <div class="modal__actions">
          <button type="button" class="btn-secondary" @click="$emit('close')">Cancel</button>
          <button type="submit" class="btn-primary" :disabled="submitting">
            {{ submitting ? "Adding…" : "Add Recipe" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import type { AddRecipePayload } from "~/types";

const emit = defineEmits(["added", "close"]);

const form = reactive<AddRecipePayload>({
  name: "",
  instructions: "",
  prepMinutes: 0,
  cookMinutes: 0,
  tags: []
});

const tagsInput = ref("");
const errors = reactive({ name: "", instructions: "" });
const submitting = ref(false);
const { pushSuccess, pushError } = useToast();

const validate = () => {
  errors.name = "";
  errors.instructions = "";
  if (!form.name.trim()) errors.name = "Name is required";
  if (!form.instructions.trim()) errors.instructions = "Instructions are required";
  return !errors.name && !errors.instructions;
};

const handleSubmit = async () => {
  if (!validate()) return;
  submitting.value = true;
  try {
    form.tags = tagsInput.value.split(",").map(t => t.trim()).filter(Boolean);
    const created = await $fetch("/api/recipes", { method: "POST", body: form });
    emit("added", created);
    pushSuccess(`"${form.name}" added!`);
  } catch (err) {
    pushError(err instanceof Error ? err : new Error("Failed to add recipe"));
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}

.modal {
  background: var(--color-surface);
  border-radius: var(--radius-lg);
  width: 90%;
  max-width: 480px;
  box-shadow: var(--shadow-lg);
  overflow: hidden;
}

.modal__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-4) var(--space-5);
  border-bottom: 1px solid var(--color-border);
}

.modal__title {
  font-family: var(--font-display);
  font-size: 18px;
  font-weight: 600;
  margin: 0;
}

.modal__close { background: none; border: none; font-size: 22px; cursor: pointer; color: var(--color-text-muted); }

.modal__form { padding: var(--space-5); display: flex; flex-direction: column; gap: var(--space-4); }

.field { display: flex; flex-direction: column; gap: var(--space-1); }
.field__label { font-size: 13px; font-weight: 600; color: var(--color-text-secondary); }
.field__input {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-3);
  font-size: 14px;
  font-family: var(--font-body);
  background: var(--color-bg);
  color: var(--color-text-primary);
  width: 100%;
  box-sizing: border-box;
}
.field__input:focus { outline: none; border-color: var(--color-accent); }
.field__error { font-size: 12px; color: var(--color-error); }

.field-row { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-3); }

.modal__actions { display: flex; justify-content: flex-end; gap: var(--space-3); margin-top: var(--space-2); }

.btn-primary {
  background: var(--color-accent);
  color: white;
  border: none;
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-5);
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background var(--transition-fast);
}
.btn-primary:hover:not(:disabled) { background: var(--color-accent-hover); }
.btn-primary:disabled { opacity: 0.5; cursor: not-allowed; }

.btn-secondary {
  background: none;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-5);
  font-size: 14px;
  cursor: pointer;
  color: var(--color-text-secondary);
}
</style>
```

- [ ] **Step 2: Write failing test**

```typescript
// src/CookHomie.Web/tests/add-recipe-modal.spec.ts
import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import AddRecipeModal from "../components/recipes/AddRecipeModal.vue";

describe("AddRecipeModal", () => {
  it("emits added event with form data on submit", async () => {
    vi.stubGlobal("useToast", () => ({ pushSuccess: vi.fn(), pushError: vi.fn() }));
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue({ id: "new", name: "Test" }));

    const wrapper = mount(AddRecipeModal, { global: { stubs: { Teleport: false } } });
    await wrapper.find('input[placeholder*="Banana"]').setValue("Test Recipe");
    await wrapper.find("form").trigger("submit");
    expect(wrapper.emitted("added")).toBeTruthy();
  });
});
```

- [ ] **Step 3: Run test — verify it fails**

Run: `cd src/CookHomie.Web && npx vitest run tests/add-recipe-modal.spec.ts`
Expected: FAIL — component not yet created

- [ ] **Step 4: Run manual smoke test**

Start Nuxt dev server, navigate to `/recipes`, click "+ Add Recipe" button, verify modal opens with form and closes on backdrop click.

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web/components/recipes/AddRecipeModal.vue src/CookHomie.Web/tests/add-recipe-modal.spec.ts
git commit -m "feat(web): add AddRecipeModal with full form and validation"
```

---

## Task 14 — Recipes index page (full)

**Files:**
- Modify: `src/CookHomie.Web/pages/recipes/index.vue`
- Create: `src/CookHomie.Web/composables/useRecipes.ts`
- Create: `src/CookHomie.Web/tests/recipes-list.spec.ts`

- [ ] **Step 1: Write useRecipes composable**

```typescript
// src/CookHomie.Web/composables/useRecipes.ts
import type { Recipe } from "~/types";

export const useRecipes = () => {
  const recipes = useState<Recipe[]>("recipes", () => []);
  const loading = useState("recipes-loading", () => false);
  const error = useState<string | null>("recipes-error", () => null);
  const isStale = useState("recipes-stale", () => false);

  let intervalId: ReturnType<typeof setInterval> | null = null;

  const fetchRecipes = async () => {
    loading.value = true;
    error.value = null;
    try {
      recipes.value = await $fetch<Recipe[]>("/api/recipes");
      isStale.value = false;
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Failed to load recipes";
      isStale.value = recipes.value.length > 0;
    } finally {
      loading.value = false;
    }
  };

  const start = () => { fetchRecipes(); intervalId = setInterval(fetchRecipes, 30000); };
  const stop = () => { if (intervalId) { clearInterval(intervalId); intervalId = null; } };
  const refresh = () => fetchRecipes();

  return { recipes, loading, error, isStale, start, stop, refresh };
};
```

- [ ] **Step 2: Write failing test**

```typescript
// src/CookHomie.Web/tests/recipes-list.spec.ts
import { describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { useRecipes } from "../composables/useRecipes";

describe("useRecipes", () => {
  beforeEach(() => {
    vi.stubGlobal("useState", (_k: string, init: () => unknown) => ref(init()));
  });

  it("fetches recipes from /api/recipes", async () => {
    const mockRecipes = [{ id: "r1", name: "Pancakes", instructions: "Mix.", prepMinutes: 5, cookMinutes: 10, tags: [] }];
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue(mockRecipes));

    const { recipes, start } = useRecipes();
    await start();

    expect(vi.mocked($fetch)).toHaveBeenCalledWith("/api/recipes");
    expect(recipes.value).toEqual(mockRecipes);
  });
});
```

- [ ] **Step 3: Run test — verify it fails**

Run: `cd src/CookHomie.Web && npx vitest run tests/recipes-list.spec.ts`
Expected: FAIL — useRecipes composable not yet created

- [ ] **Step 4: Implement full Recipes index page**

Replace `src/CookHomie.Web/pages/recipes/index.vue`:

```vue
<template>
  <div class="recipes-page">
    <header class="recipes-page__header">
      <h1 class="recipes-page__title">Recipes</h1>
      <button class="btn-add" @click="showModal = true">+ Add Recipe</button>
    </header>

    <DashboardPanel
      title=""
      :loading="loading"
      :error="error"
      :is-stale="isStale"
      :has-data="recipes.length > 0"
      show-refresh
      @refresh="refresh"
    >
      <div v-if="loading && recipes.length === 0" class="recipes-grid">
        <SkeletonBlock v-for="i in 3" :key="i" height="120px" />
      </div>
      <div v-else-if="error && recipes.length === 0">
        <ErrorBanner :message="error" show-retry @retry="refresh" />
      </div>
      <div v-else-if="recipes.length === 0" class="empty-state">
        <p>No recipes yet.</p>
      </div>
      <div v-else class="recipes-grid">
        <RecipeCard
          v-for="recipe in recipes"
          :key="recipe.id"
          :recipe="recipe"
          @click="navigateTo(`/recipes/${recipe.id}`)"
        />
      </div>
    </DashboardPanel>

    <AddRecipeModal v-if="showModal" @added="handleAdded" @close="showModal = false" />
  </div>
</template>

<script setup lang="ts">
const { recipes, loading, error, isStale, start, stop, refresh } = useRecipes();
const showModal = ref(false);

onMounted(() => start());
onUnmounted(() => stop());

const handleAdded = (recipe: { id: string }) => {
  showModal.value = false;
  navigateTo(`/recipes/${recipe.id}`);
};
</script>

<style scoped>
.recipes-page { max-width: 960px; margin: 0 auto; }

.recipes-page__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-6);
}

.recipes-page__title {
  font-family: var(--font-display);
  font-size: 28px;
  font-weight: 700;
  margin: 0;
}

.btn-add {
  background: var(--color-accent);
  color: white;
  border: none;
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-5);
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
}

.recipes-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: var(--space-4);
}

.empty-state {
  text-align: center;
  padding: var(--space-10);
  color: var(--color-text-muted);
}

@media (max-width: 480px) {
  .recipes-page__header { flex-direction: column; align-items: flex-start; gap: var(--space-3); }
  .recipes-grid { grid-template-columns: 1fr; }
}
</style>
```

- [ ] **Step 5: Verify page renders**

Start dev server: `cd src/CookHomie.Web && npm run dev`
Visit `http://localhost:3000/recipes`
Expected: Recipe grid loads, "+ Add Recipe" button works, modal opens/closes, new recipe navigates to detail

- [ ] **Step 6: Commit**

```bash
git add src/CookHomie.Web/pages/recipes/index.vue src/CookHomie.Web/composables/useRecipes.ts src/CookHomie.Web/tests/recipes-list.spec.ts
git commit -m "feat(web): implement full Recipes index page with grid and AddRecipe modal"
```
