# Milestone 2 — Design System Baseline

> **For agentic workers:** Each step is `- [ ]`. Implement task-by-task on its own branch. Commit after each task.

**Goal:** Build the shared CSS foundation — base reset/typography, shared component classes, and the base.css + components.css system.

**Prerequisite:** Complete Task 1 (design tokens CSS file created). This plan extends tokens.css into a full CSS system.

---

## Task 9 — Base CSS + Component CSS system

**Files:**
- Create: `src/CookHomie.Web/assets/css/base.css`
- Create: `src/CookHomie.Web/assets/css/components.css`
- Modify: `src/CookHomie.Web/nuxt.config.ts`

- [x] **Step 1: Write base.css**

```css
/* src/CookHomie.Web/assets/css/base.css */
@import url('https://fonts.googleapis.com/css2?family=DM+Mono&family=DM+Sans:wght@400;500;600;700&family=Literata:wght@400;600;700&display=swap');

*, *::before, *::after { box-sizing: border-box; }

html {
  font-size: 16px;
  scroll-behavior: smooth;
  -webkit-font-smoothing: antialiased;
}

body {
  margin: 0;
  font-family: var(--font-body);
  background: var(--color-bg);
  color: var(--color-text-primary);
  line-height: 1.5;
}

h1, h2, h3, h4, h5, h6 {
  font-family: var(--font-display);
  margin: 0;
  line-height: 1.2;
}

a { color: var(--color-accent); }
img { max-width: 100%; }
button { font-family: var(--font-body); }
input, textarea, select { font-family: var(--font-body); }
```

- [x] **Step 2: Write components.css**

```css
/* src/CookHomie.Web/assets/css/components.css */

/* Shared card */
.card {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
}

/* Shared buttons */
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-2);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-5);
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all var(--transition-fast);
  border: none;
}

.btn-primary { background: var(--color-accent); color: white; }
.btn-primary:hover:not(:disabled) { background: var(--color-accent-hover); }
.btn-primary:disabled { opacity: 0.5; cursor: not-allowed; }

.btn-secondary { background: none; border: 1px solid var(--color-border); color: var(--color-text-secondary); }
.btn-secondary:hover { background: var(--color-surface-hover); }

/* Shared input */
.input {
  width: 100%;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-3);
  font-size: 14px;
  background: var(--color-bg);
  color: var(--color-text-primary);
  transition: border-color var(--transition-fast);
}
.input:focus { outline: none; border-color: var(--color-accent); }
```

- [x] **Step 3: Update nuxt.config.ts**

Update the `css` array in `defineNuxtConfig`:

```ts
css: ["~/assets/css/tokens.css", "~/assets/css/base.css", "~/assets/css/components.css"]
```

- [x] **Step 4: Verify dev server**

Run: `cd src/CookHomie.Web && npm run dev`
Expected: Nuxt starts without errors; fonts load from Google Fonts CDN

- [x] **Step 5: Commit**

```bash
git add src/CookHomie.Web/assets/css/base.css src/CookHomie.Web/assets/css/components.css src/CookHomie.Web/nuxt.config.ts
git commit -m "feat(web): add base CSS and component system (tokens + base + components)"
```
