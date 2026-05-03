# Milestone 2 Frontend Completion Design (MVP)

Date: 2026-04-27  
Project: CookHomie  
Scope: Finish Milestone 2 frontend roadmap items with future-ready integration contracts and consistent UX.

## 1) Goals

Complete the remaining Milestone 2 frontend work:

1. Dashboard page (expiring items, recipe matches, shopping count)
2. Recipes page with `RecipeCard` and `AddRecipeModal`
3. Recipe detail page with ingredient in-stock highlighting

Also enforce:

- Cross-page visual/design consistency
- Mobile-first responsive behavior (phone, tablet/iPad, desktop)
- Future-ready integration through Nuxt mock API contracts shaped like planned backend endpoints

## 2) Non-Goals

- No backend production implementation of planned recipes/shopping endpoints in this milestone
- No fuzzy ingredient matching beyond exact normalized string compare
- No persistent recipe storage outside the running Nuxt mock process

## 3) Chosen Approach

Adopt a **platform-first shell** approach before feature completion:

1. Define shared UI foundation (design tokens + state UX patterns)
2. Define shared data flow patterns (composables, polling lifecycle, refresh semantics)
3. Define mock contract conventions in Nuxt `server/api`
4. Implement missing pages/components on top of this base

Reasoning: this minimizes rework, enables consistent UX, and keeps future cutover to C# API straightforward.

## 4) UX and Visual Direction

### 4.1 Design language

Use **Calm Utility** style across dashboard, inventory, recipes, and shopping:

- Cozy but practical visual tone
- Structured layouts with clear hierarchy
- Reusable card/panel patterns
- Consistent empty/loading/error states

### 4.2 Responsive strategy (mobile first)

- **Phone:** single-column by default, actions always reachable
- **Tablet/iPad:** adaptive two-column layouts where useful (cards and recipe browsing)
- **Desktop:** denser multi-column composition while preserving content hierarchy

Use shared components with responsive variants rather than separate desktop/mobile implementations unless behavior truly diverges.

## 5) Frontend Architecture and Contracts

### 5.1 Nuxt mock endpoint strategy

Use Nuxt `server/api` mock endpoints as contract layer for not-yet-implemented backend features.

Planned mock routes for Milestone 2:

- `GET /api/dashboard/summary`
- `GET /api/recipes`
- `POST /api/recipes`
- `GET /api/recipes/:id`
- (Optional parity helper) `GET /api/recipes/:id/missing`

### 5.2 AddRecipe persistence model

Use **in-memory reactive mock store** within Nuxt server layer.

- Data persists only for active process lifetime
- UI must read/write from one source of truth to avoid stale snapshots
- Restart resets data (acceptable for MVP milestone)

### 5.3 Dashboard fallback behavior

For summary blocks:

- Attempt real endpoint path when available
- If endpoint is unavailable/not implemented, fallback to mock contract data
- Show transparent state in UI where useful (without blocking usage)

## 6) Data Fetching, Polling, and Refresh

### 6.1 Polling model

- Poll every **30 seconds**
- Polling is **active-page-only** (runs only while page is mounted/visible)
- Manual refresh available on each data page

### 6.2 Page-level behavior

All data pages (Dashboard, Recipes, Inventory, Shopping) follow same lifecycle:

1. Initial fetch on mount
2. Background polling at interval
3. Manual refresh on demand
4. Stop polling on unmount/hidden

## 7) Inventory Filtering Dependency (Milestone Dependency)

Milestone 2 includes a minimal dependency on inventory filtering support to serve frontend needs cleanly.

Required filter contract:

- `location`
- `expiringBefore`

Frontend consumes this via Nuxt proxy routes so backend cutover remains isolated and does not force page rewrites.

## 8) Component and Module Design

### 8.1 New/expanded UI components

- `DashboardStatCard`
- `DashboardPanel`
- `RecipeCard`
- `AddRecipeModal`
- `IngredientStockBadge` (in-stock / missing)

### 8.2 Shared patterns to standardize

- Page frame with action bar
- Card shells and section headings
- Loading skeletons
- Empty states
- Inline error states with retry
- Stale-data indicators

## 9) Ingredient Stock Highlighting Rule

For recipe detail stock status:

- Match recipe ingredient names to inventory by exact normalized string
- Normalization: trim + case-insensitive compare
- No alias map, pluralization logic, or fuzzy distance in this milestone

This keeps logic transparent and predictable for MVP.

## 10) API Feedback and Toast Policy

Global toast system requirement:

- Parse and display API response messages for success and failure

Toast trigger policy:

- **User-triggered actions only** (create/update/delete/manual refresh)
- Background polling does not emit success/error toasts
- Polling failures are communicated via inline page/card state

## 11) Error Handling and Reliability UX

- Inline non-blocking error banners on affected modules
- Retry actions at module/page level
- Preserve last good data when refresh fails
- Show stale-data marker when appropriate
- Avoid full-page blocking where partial data can still render

## 12) Testing and Verification Strategy

Target: strong coverage without over-engineering.

### 12.1 Unit tests (broad coverage)

Prioritize composables and logic-heavy units:

- Polling lifecycle start/stop behavior
- Data state transitions (loading, success, error, stale)
- AddRecipe modal validation and submit behavior
- Ingredient stock matching utility
- Toast parsing and trigger policy

### 12.2 Automated integration/page tests (critical flows)

Automate highest-value paths:

1. Dashboard load with real-or-mock fallback behavior
2. Recipes list render and AddRecipe creation flow
3. Recipe detail in-stock highlighting
4. Polling lifecycle on mount/unmount visibility transitions

### 12.3 Responsive verification

Verify primary breakpoints for phone, tablet/iPad, desktop on key pages.

### Completed Foundation Work ✅

This milestone includes the foundation shell completed on `task/foundation-shell`:

- Design tokens (`tokens.css`) + nuxt.config.ts integration
- `usePollingFetch` composable with polling lifecycle
- `useToast` composable + ToastContainer component
- Shared UI primitives: SkeletonBlock, ErrorBanner, StaleIndicator
- Polling lifecycle added to `useInventory` composable
- Recipe types: `RecipeIngredient`, `DashboardSummary`, `AddRecipePayload`
- All tests passing (19 tests across 7 files)

## Task 02: Mock Contract Layer ✅ COMPLETED

Mock contract layer completed with commit c3e6467 and merged to main.

**Server/API Endpoints Created:**
- `GET /api/recipes` - List all recipes
- `POST /api/recipes` - Create new recipe
- `GET /api/recipes/:id` - Get single recipe
- `GET /api/dashboard/summary` - Dashboard summary data

**Recipe Store Utility:**
- In-memory recipe store managing recipe CRUD operations
- Located at `src/CookHomie.Web/server/utils/recipeStore.ts`

**Client-Side Components:**
- Ingredient stock matching utility for ingredient inventory comparison
- Located at `src/CookHomie.Web/utils/ingredientStock.ts`

All mock contract layer tasks completed, tested, and merged to main branch.

## Task 03: Design System Baseline ✅ COMPLETED

Foundation shell completed on main branch:

**CSS Infrastructure:**
- `src/CookHomie.Web/assets/css/tokens.css` - Design tokens
- `src/CookHomie.Web/assets/css/base.css` - Reset and typography
- `src/CookHomie.Web/assets/css/components.css` - Shared component styles
- Integrated into `nuxt.config.ts`

**Shared UI Primitives:**
- `SkeletonBlock.vue` - Loading placeholder
- `ErrorBanner.vue` - Error display
- `StaleIndicator.vue` - Stale data indicator
- `ToastContainer.vue` - Toast notification system

**Composables:**
- `usePollingFetch` - Polling lifecycle management (start/stop/refresh)
- `useToast` - Toast notification API
- `useInventory` - Inventory data with polling integration
- `useDevMode` - Development mode utilities

**Types:**
- Recipe, RecipeIngredient, DashboardSummary, AddRecipePayload

**Test Coverage:**
- 19 tests across composables and utilities (all passing)

## Tasks 04-07: Frontend Pages — IN PROGRESS

Remaining work for Milestone 2 completion:

- **Task 04 (Dashboard):** Page structure in place, needs DashboardStatCard/DashboardPanel components
- **Task 05 (Recipes Browser):** RecipeCard and AddRecipeForm components exist, recipes page needs integration
- **Task 06 (Recipe Detail):** Page exists, needs in-stock highlighting integration
- **Task 07 (Polish & Tests):** Responsive validation, end-to-end test coverage, final refinements

Status: Ready for implementation on feat/milestone-2-frontend branch.

## 13) Milestone 2 Completion Criteria

Milestone 2 is complete when all are true:

1. Roadmap frontend unchecked items implemented
2. Visual/design consistency baseline applied across all frontend pages
3. Mobile-first responsive behavior validated for phone/tablet/desktop
4. Nuxt mock contract layer covers required dashboard/recipes flows
5. Toast behavior matches policy (user-triggered only)
6. Unit tests and critical automated flow tests pass

## 14) Risks and Mitigations

### Risk: Contract drift between mock and future backend

Mitigation: keep route shapes and response DTOs aligned with planned API docs and centralize mock schemas.

### Risk: Polling noise and UX fatigue

Mitigation: active-page-only polling + silent background refresh + inline stale indicators.

### Risk: Temporary in-memory store confusion

Mitigation: clear dev-facing note that mock recipe writes are process-scoped and reset on restart.

## 15) Implementation Readiness Notes

This design is scoped for one implementation plan targeting Milestone 2 frontend completion, with clean handoff to later backend endpoint adoption.
