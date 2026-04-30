# Tailwind CSS Migration Decision

**Date:** 2026-04-29
**Status:** Implemented

## Decisions Made

- **Approach:** Tailwind v4, CSS-first, full migration from three custom CSS files to a single Tailwind entry point.
- **Package:** `@nuxtjs/tailwindcss` v7+ (Tailwind v4 as peer dependency).
- **Single entry point:** `assets/css/main.css` replaces `tokens.css`, `base.css`, `components.css`.
- **No config file:** Tailwind v4 is CSS-first; no `tailwind.config.js` created.

## Architecture & Boundaries

### Design Tokens (`@theme`)
"Calm Utility" palette and custom fonts preserved exactly via CSS custom properties:

| Token | Generated utilities |
|-------|-------------------|
| `--color-*` | `bg-*`, `text-*`, `border-*`, `ring-*` |
| `--font-*` | `font-*` |
| `--radius-*` | `rounded-*` |

### Component Layer (`@layer components`)
Shared classes defined in `main.css`:

- `.card` — surface background, border, rounded, shadow-sm
- `.btn`, `.btn-primary`, `.btn-secondary` — inline-flex, padding, transitions
- `.input` — full width, border, focus ring on accent

### Base Layer (`@layer base`)
Global resets for box-sizing, scroll behavior, font defaults, heading styles, link colors.

### Migration Scope
All scoped `<style>` blocks in layouts, pages, and components migrated to inline Tailwind utilities. Reused patterns moved to `@layer components`. Spacing, sizing, and shadows use Tailwind defaults exclusively.

### Nuxt Component Naming Convention
Nuxt deduplicates folder prefix when filename already starts with it. Reference table enforced across all usages:

| File | Auto-import name |
|------|-----------------|
| `dashboard/DashboardPanel.vue` | `<DashboardPanel />` |
| `dashboard/DashboardStatCard.vue` | `<DashboardStatCard />` |
| `recipes/AddRecipeForm.vue` | `<RecipesAddRecipeForm />` |
| `recipes/AddRecipeModal.vue` | `<RecipesAddRecipeModal />` |
| `recipes/RecipeCard.vue` | `<RecipesRecipeCard />` |
| `shared/*` | `<Shared* />` |

### Docs Updates
- `docs/03-RepoStructure.md` — Nuxt Component Naming section added; `assets/css/` listing updated.
- `docs/01-Architecture.md` — Frontend Styling section added documenting Tailwind v4 setup.

## Success Definition

- `npm run validate` passes in `CookHomie.Web`
- All scoped `<style>` blocks removed from Vue files
- No regressions in visual appearance (colors, fonts, border radii preserved)
- Component naming convention consistent across all Vue files