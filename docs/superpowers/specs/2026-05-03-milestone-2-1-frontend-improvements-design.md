# Milestone 2.1 — Frontend Improvements Design Spec

**Branch:** `feat/milestone2-1-frontend-improvements`

## Scope
This spec defines concrete implementation approach for nine post–Milestone 2 frontend/platform improvements across dashboard, inventory, recipes, and shopping list surfaces. Goal: increase reuse, improve UX clarity, enable shopping-list workflows, keep patterns aligned with existing Shared components + composables architecture.

## Cross-Cutting Decisions (Affect Multiple Items)
- **Shared paginated ingredient table** (Imp #7 + #8): build one reusable component now, not two local versions.
- **Shopping list deduplication contract** (Imp #6 + #8 + #9): dedupe by normalized ingredient/item name + `isBought === false`, optionally scoped by `linkedRecipeId` metadata, but uniqueness check centered on active-name match.
- **Shopping API availability** (Imp #9 unblocks #6 + #8): missing-ingredient add flows must wait for shopping endpoints/composable support.
- **Canonical date formatting utility** (Imp #2 + dashboard/inventory consistency): centralize core day-diff logic in `utils/date.ts`, allow view-level formatting variants.
- **Modal interaction model** (Imp #5 + dashboard expiring panel): one base item modal with mode/action extensions; avoid duplicate modal stacks.

---

## 1) Shared Refresh Button
### Problem Summary
Refresh button markup duplicated in `pages/index.vue` and `components/dashboard/DashboardPanel.vue`, creating drift risk and harder style/behavior updates.

### Approaches
1. **Strict presentational `RefreshButton` (recommended)**
   - Props: `loading?: boolean`, `label?: string` (default "Refresh"), optional `size` if needed later.
   - Emits: `refresh` only.
   - Keep `showRefresh` conditional at call site.
   - **Trade-off:** Caller writes conditionals; component stays clean/single-purpose.

2. **Self-conditional component**
   - Add prop `visible?: boolean`; component returns nothing if false.
   - **Trade-off:** Fewer call-site conditionals; but UI visibility logic hidden inside child, weaker readability.

3. **Generic shared action button wrapper**
   - Build broader abstraction (icon + loading + action types).
   - **Trade-off:** Future-flexible, but over-abstracted for one clear use case.

### Open Questions / Dependencies
- Should label remain fixed for consistency or customizable per panel?
- If iconography changes globally, should this component own icon import?

### Recommendation
Use **Approach 1**. Keep `showRefresh` at call sites for explicit layout control; component handles only rendering + disabled/loading behavior.

---

## 2) Fix `formatExpiry` with Calendar-Day Comparison
### Problem Summary
Two slightly different `formatExpiry` implementations produce inconsistent user messaging and likely off-by-one behavior around timezone boundaries.

### Approaches
1. **Canonical utility in `utils/date.ts` + formatter variants (recommended)**
   - Core fn computes day delta using local calendar day boundaries (`startOfLocalDay(expiry) - startOfLocalDay(now)`).
   - Expose shared semantic output enum/state (`expired`, `today`, `tomorrow`, `inNDays`).
   - Views map semantic state to long/short strings.
   - **Trade-off:** Slightly more structure; best consistency + flexibility.

2. **Single string formatter for all pages**
   - One function returns final copy used everywhere.
   - **Trade-off:** Max consistency; weaker UX nuance (inventory short form vs dashboard descriptive form).

3. **Leave per-page formatter, share only helper**
   - Shared `getDayDiff`, local message builders.
   - **Trade-off:** Lower migration friction; still some duplication risk.

### Open Questions / Dependencies
- Kitchen app timezone source: browser local time vs explicit user setting (MVP likely browser local).
- Should “Expires in 3d” remain for dense table contexts?

### Recommendation
Use **Approach 1**. Centralize day math in `utils/date.ts`; keep page-specific copy mapping (short inventory label preserved, dashboard can remain verbose).

Suggested test cases:
- Expiry same day near midnight.
- Expiry tomorrow with DST transition.
- Already expired yesterday.
- Null/undefined expiry.

---

## 3) API URL Constants File
### Problem Summary
Composable endpoints currently hardcoded ad hoc, making route drift and typo regressions more likely as API surface grows.

### Approaches
1. **Namespaced `API_ROUTES` module (recommended)**
   - File: `utils/apiRoutes.ts`.
   - Shape:
     - `DASHBOARD.SUMMARY`
     - `INVENTORY.LIST`
     - `RECIPES.LIST`, `RECIPES.DETAIL(id)`, `RECIPES.MISSING(id)`
     - `SHOPPING.LIST`, etc.
   - **Trade-off:** Slight verbosity; strongest discoverability + scaling.

2. **Flat `API_URLS` object**
   - Keys like `DASHBOARD_SUMMARY`, `RECIPES_DETAIL` fn.
   - **Trade-off:** Simple; degrades readability as domains grow.

3. **Co-located constants per composable**
   - Keep constants in each composable file.
   - **Trade-off:** Locality high; duplication remains.

### Open Questions / Dependencies
- Align naming with server route conventions now to avoid future churn.
- Shopping routes (#9) should be added same pass for consistency.

### Recommendation
Use **Approach 1** with domain namespaces + function entries for parameterized routes.

---

## 4) Recipe Name as Anchor on Dashboard
### Problem Summary
Quick Recipe Ideas shows recipe name as non-interactive `<span>`, blocking expected navigation to detail page.

### Approaches
1. **Name-only `NuxtLink` with subtle link affordance (recommended)**
   - Keep row structure; only name clickable.
   - Style mostly neutral with hover underline/focus ring for accessibility.
   - **Trade-off:** Precise interaction target; safest with other row actions (#6).

2. **Whole row clickable card**
   - Entire idea row acts as link.
   - **Trade-off:** Bigger target; conflicts with inline secondary actions and nested buttons.

3. **Button-style CTA beside name**
   - Keep text static, add “View” link/button.
   - **Trade-off:** Explicit; adds visual clutter.

### Open Questions / Dependencies
- With missing-ingredient CTA (#6), whole-row click likely problematic.
- Existing hover styles may need split: row hover + link-specific focus.

### Recommendation
Use **Approach 1**. Link on recipe name only; preserve composability for additional actions.

---

## 5) Inventory Item Click → View/Edit Modal
### Problem Summary
Inventory rows and dashboard expiring items imply interactivity (hover) but no click behavior. Users cannot quickly inspect/update item lifecycle states.

### Approaches
1. **Single reusable `ItemDetailModal` with context mode (recommended)**
   - Modes: `inventory` and `expiring`.
   - Shared core: view + edit fields.
   - Expiring-only contextual actions: dismiss/discard-leftover, already-restocked, add-to-shopping-list.
   - **Trade-off:** Slightly richer props/events; avoids duplicated modal implementations.

2. **Two separate modals**
   - `InventoryItemModal` + `ExpiringItemActionModal`.
   - **Trade-off:** Simpler per file; duplicated form/state logic likely.

3. **Drawer/side panel instead of modal**
   - Better multitask flow.
   - **Trade-off:** New interaction paradigm inconsistent with existing SharedModal.

### Open Questions / Dependencies
- Edit fields parity with `AddItemModal`: recommend full parity except immutable ID.
- “Dismiss” semantics: should suppress reminder state (not delete). Needs backend support flag or local-only suppression policy.
- “Already restocked”: update quantity/expiry quickly or just close + toast?

### Recommendation
Use **Approach 1**. One modal component extended by mode-specific action slot/section. Keep UX consistent with SharedModal pattern.

State update strategy:
- Successful edit/action triggers composable refresh + success toast.
- Optimistic close allowed for fast feel; rollback toast on failure.

---

## 6) Add Missing Recipe Ingredients to Shopping List (Dashboard)
### Problem Summary
Dashboard surfaces missing counts but lacks direct workflow to inspect/add missing ingredients, forcing context switch to recipe detail.

### Approaches
1. **Inline “View missing” expand + bulk add CTA (recommended)**
   - Per idea action loads missing list on demand (`recipes/[id]/missing`).
   - Show checklist/table + “Add all missing”.
   - Optional per-row add buttons.
   - **Trade-off:** More UI complexity; best task completion from dashboard.

2. **Single “Add missing” one-click button**
   - Fetch + bulk add immediately.
   - **Trade-off:** Fast; low transparency/control.

3. **Only navigate to recipe detail for action**
   - Keep dashboard minimal.
   - **Trade-off:** Simplest; misses requested dashboard workflow.

### Open Questions / Dependencies
- Depends on shopping API + composable support (#9).
- Dedup rule must match detail page behavior (#8).
- Interaction with linked recipe navigation (#4): separate controls needed.

### Recommendation
Use **Approach 1** with lazy-load missing list + bulk add primary action. Show toast for per-item duplicates (“already on shopping list”).

---

## 7) AddRecipeModal: Paginated Ingredients Table + Time Validation
### Problem Summary
Ingredient list in modal grows vertically and hurts usability; time fields allow invalid zero values.

### Approaches
1. **Adopt shared `IngredientTable` component now + local pagination state (recommended)**
   - 5 rows/page default.
   - Modal owns pagination state and editing handlers.
   - Validate `prepMinutes` and `cookMinutes` >= 1 before submit and inline error display.
   - **Trade-off:** Initial component design effort; strong reuse with #8.

2. **Local table implementation in modal first**
   - Build simple paginated table only in `AddRecipeModal`.
   - **Trade-off:** Fast short-term; duplicate rework for #8.

3. **Collapse/accordion per ingredient row**
   - No pagination.
   - **Trade-off:** Fewer controls; still long scroll with many items.

### Open Questions / Dependencies
- Shared table API must support editable + read-only modes for #8.
- Validation messaging should match existing `FormField` error style conventions.

### Recommendation
Use **Approach 1**. Build reusable ingredient table immediately; keep modal pagination local and simple.

---

## 8) Recipe Detail: Shared Ingredient Table + Shopping Actions + Inline Edit
### Problem Summary
Recipe detail currently read-centric; lacks integrated shopping actions and in-place editing workflow, reducing efficiency for iterative recipe maintenance.

### Approaches
1. **Section-level edit mode + shared table slots (recommended)**
   - Small edit toggle per section (meta, instructions, ingredients) or global “Edit recipe” mode.
   - Input-swap pattern (display text ↔ form controls), not `contenteditable`.
   - Shared ingredient table used read-only and editable via slots/props.
   - Per-missing-row “Add to shopping list” action with dedup toast.
   - **Trade-off:** More state management; highest control/accessibility/maintainability.

2. **Fully inline micro-edit on click field-by-field**
   - Click any field to edit immediately.
   - **Trade-off:** Feels native; complex focus/save/cancel/error flows.

3. **Hybrid: detail page view + embedded mini-modal editors**
   - Keep page mostly static, modalize edits.
   - **Trade-off:** Easier logic; violates “should feel native, not separate modal.”

### Open Questions / Dependencies
- PATCH contract required in API (`recipes/:id` partial updates) including ingredient list mutations.
- Optimistic update policy: recommend optimistic UI for scalar fields, conservative round-trip for ingredient structural changes.
- Dedup policy shared with #6/#9.

### Recommendation
Use **Approach 1** with explicit edit mode toggles and controlled form inputs. Avoid `contenteditable`; too fragile for validation and keyboard support.

PATCH endpoint guidance:
- Support partial fields.
- Include ingredients delta or full replacement (prefer full replacement MVP for simplicity + deterministic client sync).

---

## 9) Shopping List API — Models, Interfaces, Services
### Problem Summary
Frontend composable exists, but no `server/api/shopping/*` routes or backend contract, blocking key shopping workflows from recipes/dashboard.

### Approaches
1. **Full CRUD + bulk-add endpoint (recommended)**
   - Routes:
     - `GET /api/shopping`
     - `POST /api/shopping`
     - `PATCH /api/shopping/:id`
     - `DELETE /api/shopping/:id`
     - `POST /api/shopping/bulk` (missing ingredients import)
   - `linkedRecipeId` optional UUID/string reference (soft link MVP).
   - Priority enum maintained as `low|medium|high` across layers.
   - **Trade-off:** Larger initial scope; unblocks #6/#8 cleanly.

2. **Minimal CRUD first, no bulk endpoint**
   - Bulk handled client-side loop of POSTs.
   - **Trade-off:** Faster backend start; less efficient and weaker transactional behavior.

3. **Only GET + POST in MVP**
   - Delay patch/delete.
   - **Trade-off:** Not sufficient for expected list management UX.

### Open Questions / Dependencies
- FK enforcement for `linkedRecipeId`: hard FK gives integrity, but recipe deletion complexity. MVP recommendation: soft reference + nullable.
- Dedup enforcement layer: server should guard duplicates to prevent race conditions; client pre-check for UX.
- `useShoppingList` API should expose `addItem`, `addBulk`, `removeItem`, `updateItem`, `toggleBought`, `refresh`.

### Recommendation
Use **Approach 1**. Implement complete CRUD + bulk import now; this is foundational dependency for recipe/dashboard shopping flows.

---

## Recommended Sequencing
1. #9 Shopping API foundation.
2. #3 API routes constants (include shopping routes).
3. #2 canonical date util.
4. #1 shared refresh button.
5. #7 shared ingredient table + AddRecipeModal validation.
6. #8 recipe detail inline edit + shopping row actions.
7. #4 recipe name link polish.
8. #6 dashboard missing ingredient workflow.
9. #5 inventory + expiring item modal actions.

Rationale: unblock dependencies first (#9), then shared primitives (#3/#2/#1/#7), then feature surfaces (#8/#6/#5).

## Branch Plan
- Milestone branch for all tasks from this spec:
  - `feat/milestone2-1-frontend-improvements`

## Tasks
- [ ] Task 1: Extract `components/shared/RefreshButton.vue` and migrate dashboard/index usages.
- [ ] Task 2: Centralize expiry day-diff logic in `utils/date.ts` and map short/long copy per view.
- [ ] Task 3: Add `utils/apiRoutes.ts` namespaced constants/functions and migrate composables.
- [ ] Task 4: Convert dashboard quick recipe name from span to accessible `NuxtLink`.
- [ ] Task 5: Build reusable inventory item detail/edit modal with expiring-context actions.
- [ ] Task 6: Add dashboard missing-ingredient expansion and bulk/per-item shopping actions.
- [ ] Task 7: Introduce shared paginated ingredient table and integrate into `AddRecipeModal`.
- [ ] Task 8: Upgrade recipe detail with shared ingredient table, per-row shopping add, section inline editing, and PATCH integration.
- [ ] Task 9: Implement shopping list API routes/contracts/services + composable operations, including bulk add and dedup guards.

# FOR THE USER
Execution order (per spec dependencies): #9 → #3 → #2 → #1 → #7 → #8 → #4 → #6 → #5
Two execution options available when ready:
1. Subagent-Driven — fresh subagent per task, review between tasks
2. Inline Execution — batch with checkpoints in this session
