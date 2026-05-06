Validate: Task 1 — Recipes API (feat(api+web): persist recipes via C# API, replace mock with proxies)
Setup
- [ ] C# API running on http://localhost:5000
- [ ] Nuxt dev server running on http://localhost:3000
- [ ] Database has at least 1 inventory item seeded
---
Happy Path — Backend (C# API via Postman/curl)
1. GET /api/recipes → 200 with empty array or list of persisted recipes
2. POST /api/recipes with valid body → 201 at /api/recipes/{id} with full recipe + ingredients
3. GET /api/recipes/{id} with known ID → 200 with recipe object
4. PATCH /api/recipes/{id} with partial body → 200 with updated recipe, ingredients replaced
5. GET /api/recipes/{id}/missing → 200 with string[] of missing ingredient names
---
Edge Cases — Backend
1. POST /api/recipes with blank name → 400 ProblemDetails
2. POST /api/recipes with missing instructions → 400 ProblemDetails
3. POST /api/recipes with ingredient quantity <= 0 → 400 ProblemDetails
4. GET /api/recipes/{id} with unknown GUID → 404 ProblemDetails "Recipe not found"
5. PATCH /api/recipes/{id} with unknown GUID → 404 ProblemDetails
6. GET /api/recipes/{id}/missing with unknown GUID → 404 ProblemDetails
7. PATCH /api/recipes/{id} with new ingredient list → ingredients fully replaced (old ones removed)
---
Happy Path — Frontend (browser)
1. Navigate /recipes → recipe list loads (no mock data indicator)
2. Click recipe card → /recipes/{id} loads with correct data from API
3. Add Recipe modal: fill all fields + 1 ingredient → submit → recipe appears in list
4. Edit recipe name/tags → PATCH called → changes reflect immediately
5. On /recipes/{id}, click to expand missing ingredients → GET /api/recipes/{id}/missing called → list shows
6. On dashboard (/), expiring item card → click item → InventoryItemDetailModal opens with populated item (not empty)
---
Edge Cases — Frontend
1. Recipe with 0 ingredients → POST → backend returns 400 (not crash)
2. Inventory has no items → GET /api/recipes/{id}/missing returns all required ingredient names
3. Expiring item clicked with no inventory refresh delay → modal still gets correct item (null guard via localInventoryItem computed)
---
Regressions
1. /inventory page → inventory CRUD still works end-to-end
2. /shopping page → shopping list CRUD still works
3. Dashboard stats cards (expiringCount, recipeMatchCount, shoppingCount) → still render and poll
4. recipeStore.ts deleted → no import errors anywhere in codebase
5. recipe-store.spec.ts deleted → no dangling test references
6. dotnet test CookHomie.sln → still 20/20 passing
---
Cleanup
- [ ] Remove any test recipe added during validation
- [ ] Restore console.log lines if any were re-added during conflict resolution
