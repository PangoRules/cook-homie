# Tailwind CSS Migration Design

**Date:** 2026-04-29
**Status:** Approved
**Approach:** Tailwind v4, CSS-first, full migration

---

## 1. Goals

- Replace the three custom CSS files (`tokens.css`, `base.css`, `components.css`) with a single Tailwind v4 entry point.
- Migrate all scoped `<style>` blocks in components and pages to Tailwind utility classes.
- Fix Nuxt auto-import naming inconsistencies across the frontend.
- Update project docs to reflect the new styling system and naming rules.
- Preserve the existing "Calm Utility" color palette and custom fonts exactly. Use Tailwind defaults for spacing, sizing, and shadows.

---

## 2. Setup

### Package

```bash
npm install -D @nuxtjs/tailwindcss
```

Tailwind v4 is a peer dependency of `@nuxtjs/tailwindcss` v7+ and is installed automatically.

### `nuxt.config.ts`

```ts
export default defineNuxtConfig({
  modules: ['@nuxt/eslint', '@nuxtjs/tailwindcss'],
  css: ['~/assets/css/main.css'],
  // ...rest unchanged
})
```

The three existing `css` entries (`tokens.css`, `base.css`, `components.css`) are replaced by the single `main.css`.

### File structure delta

```
assets/css/
  main.css        ← new (owns everything)
  tokens.css      ← deleted
  base.css        ← deleted
  components.css  ← deleted
```

---

## 3. `assets/css/main.css`

The full file, in order:

```css
@import "tailwindcss";
@import url('https://fonts.googleapis.com/css2?family=DM+Mono&family=DM+Sans:wght@400;500;600;700&family=Literata:wght@400;600;700&display=swap');

/* ─── Design tokens ─────────────────────────────────── */
@theme {
  /* Colors — "Calm Utility" palette */
  --color-bg:             #fafaf8;
  --color-surface:        #ffffff;
  --color-surface-hover:  #f5f4f0;
  --color-border:         #e8e6e1;
  --color-border-strong:  #ccc9c3;
  --color-text-primary:   #1a1918;
  --color-text-secondary: #6b6860;
  --color-text-muted:     #9c9991;
  --color-accent:         #c8602a;
  --color-accent-hover:   #b5541f;
  --color-accent-subtle:  #fdf0e9;
  --color-success:        #3d7a4f;
  --color-success-subtle: #edf5f0;
  --color-warning:        #a06723;
  --color-warning-subtle: #fff3e0;
  --color-error:          #b83232;
  --color-error-subtle:   #fef2f2;
  --color-info:           #2563a8;
  --color-info-subtle:    #eff6ff;

  /* Fonts */
  --font-display: "Literata", Georgia, serif;
  --font-body:    "DM Sans", system-ui, sans-serif;
  --font-mono:    "DM Mono", monospace;

  /* Border radius (preserved — larger/softer than Tailwind defaults) */
  --radius-sm:   6px;
  --radius-md:   10px;
  --radius-lg:   16px;
  --radius-full: 9999px;
}

/* ─── Base layer ─────────────────────────────────────── */
@layer base {
  *, *::before, *::after { box-sizing: border-box; }

  html  { @apply scroll-smooth antialiased text-base; }
  body  { @apply font-body bg-bg text-text-primary leading-normal m-0; }

  h1, h2, h3, h4, h5, h6 { @apply font-display leading-tight m-0; }

  a                        { @apply text-accent; }
  img                      { @apply max-w-full; }
  button, input, textarea, select { @apply font-body; }
}

/* ─── Shared component classes ───────────────────────── */
@layer components {
  .card {
    @apply bg-surface border border-border rounded-lg shadow-sm;
  }

  .btn {
    @apply inline-flex items-center justify-center gap-2
           rounded-md px-5 py-2 text-sm font-semibold
           cursor-pointer transition-all border-0;
  }
  .btn-primary {
    @apply bg-accent text-white
           hover:bg-accent-hover
           disabled:opacity-50 disabled:cursor-not-allowed;
  }
  .btn-secondary {
    @apply bg-transparent border border-border text-text-secondary
           hover:bg-surface-hover;
  }

  .input {
    @apply w-full border border-border rounded-md px-3 py-2
           text-sm bg-bg text-text-primary
           transition-colors outline-none focus:border-accent;
  }
}
```

**What each `@theme` token generates:**

| Token | Generated utilities |
|-------|-------------------|
| `--color-accent` | `bg-accent`, `text-accent`, `border-accent`, `ring-accent` |
| `--color-text-primary` | `bg-text-primary`, `text-text-primary`, `border-text-primary` |
| `--font-display` | `font-display` |
| `--radius-md` | `rounded-md` |
| *(all others follow same pattern)* | |

Spacing (`p-4`, `gap-2`, etc.), shadows (`shadow-sm`), and sizing use Tailwind defaults. Transition tokens (`--transition-fast` etc.) are removed — use `transition-all duration-150` / `duration-200` inline.

---

## 4. Component migration

Every Vue file with a scoped `<style>` block gets migrated:

1. Read the scoped CSS rules.
2. Map each rule to Tailwind utilities applied directly in the template.
3. Delete the `<style scoped>` block.
4. One-off layout classes (used only in that file) become inline utilities. Reused patterns (`.btn`, `.card`, `.input`) stay as `@layer components` classes in `main.css`.

### Files to migrate

| File | Has `<style>`? |
|------|---------------|
| `layouts/default.vue` | yes |
| `components/dashboard/DashboardPanel.vue` | yes |
| `components/dashboard/DashboardStatCard.vue` | yes |
| `components/recipes/AddRecipeForm.vue` | yes |
| `components/recipes/AddRecipeModal.vue` | yes |
| `components/recipes/RecipeCard.vue` | yes |
| `components/shared/ErrorBanner.vue` | yes |
| `components/shared/SkeletonBlock.vue` | yes |
| `components/shared/StaleIndicator.vue` | yes |
| `components/shared/ToastContainer.vue` | yes |
| `pages/index.vue` | yes |
| `pages/development.vue` | yes |
| `pages/recipes.vue` | yes |
| `pages/recipes/[id].vue` | yes |
| `pages/recipes/index.vue` | yes |
| `pages/shopping.vue` | yes |

---

## 5. Nuxt auto-import naming convention

### The rule

Nuxt generates the component name from the full path relative to `components/`:

```
components/<Folder>/<ComponentName>.vue → <Folder><ComponentName>
```

**Exception (deduplication):** if the filename already starts with the folder name (case-insensitive), Nuxt drops the folder prefix.

```
components/dashboard/DashboardPanel.vue → <DashboardPanel />     ✅ deduplicated
components/shared/StaleIndicator.vue    → <SharedStaleIndicator /> ✅ prefix added
```

### Known incorrect usages to fix

| Current (wrong) | Correct |
|----------------|---------|
| `<RecipeCard />` | `<RecipesRecipeCard />` |
| `<AddRecipeModal />` | `<RecipesAddRecipeModal />` |

All `shared/*` and `dashboard/*` usages already use the correct prefixed names after recent fixes.

### Correct reference table (full)

| File | Auto-import name |
|------|-----------------|
| `shared/DevModeGuard.vue` | `<SharedDevModeGuard />` |
| `shared/ErrorBanner.vue` | `<SharedErrorBanner />` |
| `shared/SkeletonBlock.vue` | `<SharedSkeletonBlock />` |
| `shared/StaleIndicator.vue` | `<SharedStaleIndicator />` |
| `shared/ToastContainer.vue` | `<SharedToastContainer />` |
| `dashboard/DashboardPanel.vue` | `<DashboardPanel />` |
| `dashboard/DashboardStatCard.vue` | `<DashboardStatCard />` |
| `recipes/AddRecipeForm.vue` | `<RecipesAddRecipeForm />` |
| `recipes/AddRecipeModal.vue` | `<RecipesAddRecipeModal />` |
| `recipes/RecipeCard.vue` | `<RecipesRecipeCard />` |
| `inventory/AddItemModal.vue` | `<InventoryAddItemModal />` |

---

## 6. Docs updates

Two docs files are updated as part of this work (not a separate task):

### `docs/03-RepoStructure.md`
- Add **Nuxt Component Naming** section with the rule and reference table above.
- Update the `assets/css/` listing to show only `main.css`.
- Update the shared component listing to show correct auto-import names.

### `docs/01-Architecture.md`
- Add **Frontend Styling** section documenting:
  - Tailwind v4 via `@nuxtjs/tailwindcss`
  - `assets/css/main.css` as the single style entry point
  - `@theme` for custom colors and fonts
  - `@layer components` for shared classes (`.btn`, `.card`, `.input`)
  - Tailwind defaults used for spacing, sizing, shadows

---

## 7. What is NOT changing

- No new components are added.
- No composable or API logic is touched.
- Visual appearance is preserved — same colors, same fonts, same border radii.
- `nuxt.config.ts` changes are limited to adding the module and swapping the CSS array.
- No `tailwind.config.js` is created — Tailwind v4 is CSS-first.
