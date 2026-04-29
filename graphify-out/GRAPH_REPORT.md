# Graph Report - .  (2026-04-29)

## Corpus Check
- Corpus is ~24,954 words - fits in a single context window. You may not need a graph.

## Summary
- 333 nodes · 320 edges · 42 communities detected
- Extraction: 91% EXTRACTED · 9% INFERRED · 0% AMBIGUOUS · INFERRED: 30 edges (avg confidence: 0.78)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Nuxt API Routes|Nuxt API Routes]]
- [[_COMMUNITY_Inventory Persistence|Inventory Persistence]]
- [[_COMMUNITY_Recipe Store|Recipe Store]]
- [[_COMMUNITY_MCP API Tools|MCP API Tools]]
- [[_COMMUNITY_Use Case Tests|Use Case Tests]]
- [[_COMMUNITY_Web API Controllers|Web API Controllers]]
- [[_COMMUNITY_EF Migrations|EF Migrations]]
- [[_COMMUNITY_API Client Config|API Client Config]]
- [[_COMMUNITY_Development Page|Development Page]]
- [[_COMMUNITY_Recipe Composables|Recipe Composables]]
- [[_COMMUNITY_Controller Tests|Controller Tests]]
- [[_COMMUNITY_System Architecture|System Architecture]]
- [[_COMMUNITY_Domain Data Model|Domain Data Model]]
- [[_COMMUNITY_Inventory Use Case|Inventory Use Case]]
- [[_COMMUNITY_Dashboard Polling|Dashboard Polling]]
- [[_COMMUNITY_Model Snapshot|Model Snapshot]]
- [[_COMMUNITY_Repository Interface|Repository Interface]]
- [[_COMMUNITY_Seed Migration|Seed Migration]]
- [[_COMMUNITY_App DbContext|App DbContext]]
- [[_COMMUNITY_DbContext Factory|DbContext Factory]]
- [[_COMMUNITY_Repository Tests|Repository Tests]]
- [[_COMMUNITY_Dev Mode Guard|Dev Mode Guard]]
- [[_COMMUNITY_Project Overview|Project Overview]]
- [[_COMMUNITY_Polling Lifecycle|Polling Lifecycle]]
- [[_COMMUNITY_Add Inventory Use Case|Add Inventory Use Case]]
- [[_COMMUNITY_Test DB Factory|Test DB Factory]]
- [[_COMMUNITY_Inventory Composable|Inventory Composable]]
- [[_COMMUNITY_Ingredient Matching|Ingredient Matching]]
- [[_COMMUNITY_Inventory Entity|Inventory Entity]]
- [[_COMMUNITY_Recipe Entity|Recipe Entity]]
- [[_COMMUNITY_Recipe Ingredient|Recipe Ingredient]]
- [[_COMMUNITY_Shopping Entity|Shopping Entity]]
- [[_COMMUNITY_Add Inventory DTO|Add Inventory DTO]]
- [[_COMMUNITY_Inventory DTO|Inventory DTO]]
- [[_COMMUNITY_Program Startup|Program Startup]]
- [[_COMMUNITY_Environment Types|Environment Types]]
- [[_COMMUNITY_Recipe Detail|Recipe Detail]]
- [[_COMMUNITY_Shopping List|Shopping List]]
- [[_COMMUNITY_Frontend Milestone|Frontend Milestone]]
- [[_COMMUNITY_Hello Controller|Hello Controller]]
- [[_COMMUNITY_Recipe Detail Route|Recipe Detail Route]]
- [[_COMMUNITY_Recipe Index Route|Recipe Index Route]]

## God Nodes (most connected - your core abstractions)
1. `AppDbContext` - 12 edges
2. `get_inventory()` - 9 edges
3. `AddInventoryItemUseCaseTests` - 7 edges
4. `InventoryControllerTests` - 7 edges
5. `IInventoryRepository` - 7 edges
6. `ApiClient` - 6 edges
7. `Seed Inventory Baseline` - 6 edges
8. `useInventory` - 6 edges
9. `default.vue layout` - 6 edges
10. `Recipe` - 6 edges

## Surprising Connections (you probably didn't know these)
- `GetPollingData` --semantically_similar_to--> `30 Second Polling Lifecycle`  [INFERRED] [semantically similar]
  src/CookHomie.Api/Controllers/PollingController.cs → docs/superpowers/specs/2026-04-27-milestone-2-frontend-design.md
- `Nuxt Runtime API Base URL` --semantically_similar_to--> `API Base URL Configuration`  [INFERRED] [semantically similar]
  CookHomie.Web/nuxt.config.ts → CookHomie.MCP/api_client.py
- `/api/inventory Endpoint` --references--> `AddInventoryItemUseCase`  [EXTRACTED]
  docs/03-RepoStructure.md → src/CookHomie.Api/CookHomie.Application/UseCases/Inventory/AddInventoryItemUseCase.cs
- `AppDbContext` --references--> `InventoryItem Entity`  [EXTRACTED]
  docs/decisions/efcore-infra.md → src/CookHomie.Api/CookHomie.Domain/Entities/InventoryItem.cs
- `AppDbContext` --references--> `Recipe Entity`  [EXTRACTED]
  docs/decisions/efcore-infra.md → src/CookHomie.Api/CookHomie.Domain/Entities/Recipe.cs

## Hyperedges (group relationships)
- **Clean Architecture Layers** — 01_architecture_clean_architecture, addinventoryitemusecase_addinventoryitemusecase, inventoryitem_inventoryitem, efcore_infra_appdbcontext, 03_repostructure_api_inventory_endpoint [EXTRACTED 1.00]
- **V1 Data Model Entities** — inventoryitem_inventoryitem, recipe_recipe, recipeingredient_recipeingredient, shoppingitem_shoppingitem [EXTRACTED 1.00]
- **Milestone 2 Recipes Frontend** — 2026_04_27_task_05_recipes_browser_recipecard, 2026_04_27_task_05_recipes_browser_addrecipemodal, 2026_04_27_task_05_recipes_browser_userecipes, 2026_04_27_task_06_recipe_detail_ingredientstockbadge, 2026_04_27_task_06_recipe_detail_userecipedetail [EXTRACTED 1.00]
- **Inventory Vertical Slice** — inventorycontroller_inventorycontroller, inventorycontroller_get, inventorycontroller_post, iinventoryrepository_iinventoryrepository, inventoryrepository_inventoryrepository, appdbcontext_appdbcontext, appdbcontext_inventoryitems [INFERRED 0.86]
- **Inventory Baseline Seed Flow** — appdbcontext_seed_inventory_baseline, seedinventorybaselinedata_seedinventorybaselinedata, program_startup_seed_inventory, appdbcontextmodelsnapshot_appdbcontextmodelsnapshot [INFERRED 0.84]
- **Inventory Test Coverage** — addinventoryitemusecasetests_addinventoryitemusecasetests, inventoryrepositorytests_inventoryrepositorytests, inventorycontrollertests_inventorycontrollertests, testdbfactory_testdbfactory [INFERRED 0.82]
- **MCP Tool Surface** — tools_inventory_get_inventory, tools_recipes_get_recipes, tools_shopping_get_shopping_list, system_cookhomie_mcp_system_prompt [INFERRED 0.78]
- **Dashboard Loading Error Stale States** — cookhomie_web_components_dashboard_dashboardpanel_vue, cookhomie_web_components_shared_skeletonblock_vue, cookhomie_web_components_shared_errorbanner_vue, cookhomie_web_components_shared_staleindicator_vue [EXTRACTED 1.00]
- **Recipe Creation And Shopping Flow** — recipes_addrecipeform_submitform, recipes_recipecard_addtoshoppinglist, composables_userecipes_userecipes, composables_useshoppinglist_useshoppinglist, cookhomie_recipe_type [INFERRED 0.74]

## Communities

### Community 0 - "Nuxt API Routes"
Cohesion: 0.1
Nodes (25): config.apiBaseUrl, /api/dashboard/summary, /api/dev/inventory-seed, /api/inventory, /api/polling-data, /api/recipes, Nuxt defineEventHandler, Nuxt useState (+17 more)

### Community 1 - "Inventory Persistence"
Cohesion: 0.1
Nodes (28): AddInventoryItemUseCaseTests, FakeInventoryRepository, AppDbContext, InventoryItems DbSet, Seed Inventory Baseline, AppDbContextFactory, AppDbContextModelSnapshot, IInventoryRepository (+20 more)

### Community 2 - "Recipe Store"
Cohesion: 0.11
Nodes (16): Avocado Toast seed recipe, Classic Pancakes seed recipe, Tomato Pasta seed recipe, AddInventoryItemPayload, AddRecipePayload, AddShoppingItemPayload, AppEnv, DashboardSummary (+8 more)

### Community 3 - "MCP API Tools"
Cohesion: 0.13
Nodes (12): GET /api/inventory, api_inventory(), api_recipes(), api_shopping(), CookHomie MCP System Prompt, test_get_inventory_handles_empty_result(), test_get_inventory_passes_location_filter(), test_get_inventory_returns_items_dict() (+4 more)

### Community 4 - "Use Case Tests"
Cohesion: 0.12
Nodes (4): AddInventoryItemUseCaseTests, FakeInventoryRepository, IInventoryRepository, InventoryRepository

### Community 5 - "Web API Controllers"
Cohesion: 0.13
Nodes (7): ControllerBase, HelloController, HelloResponse, InventoryController, InventoryItemResponse, CookHomie.Api.Controllers, PollingController

### Community 6 - "EF Migrations"
Cohesion: 0.15
Nodes (5): Migration, InitialCreate, InitialCreate, CookHomie.Infrastructure.Migrations, SeedInventoryBaselineData

### Community 7 - "API Client Config"
Cohesion: 0.21
Nodes (7): API Base URL Configuration, GET /api/hello, ApiClient, _build_api_client(), hello_world(), _hello_world_with_client(), Nuxt Runtime API Base URL

### Community 8 - "Development Page"
Cohesion: 0.2
Nodes (3): addPollingLog(), startPollingData(), stopPollingData()

### Community 9 - "Recipe Composables"
Cohesion: 0.2
Nodes (6): useRecipes(), useShoppingList(), useToast(), Recipe Type, submitForm(), addToShoppingList()

### Community 10 - "Controller Tests"
Cohesion: 0.2
Nodes (4): CreateInventoryItemRequest, InventoryControllerTests, InventoryItemResponse, IClassFixture

### Community 11 - "System Architecture"
Cohesion: 0.2
Nodes (10): Clean Architecture, C# ASP.NET API, Nuxt 3 Web, PostgreSQL, Python MCP Server, /api/inventory Endpoint, get_inventory MCP Tool, AddInventoryItemUseCase (+2 more)

### Community 12 - "Domain Data Model"
Cohesion: 0.25
Nodes (9): InventoryItem, Name-Based Ingredient Matching, Recipe, RecipeIngredient, ShoppingItem, Tags as PostgreSQL text[], AddRecipeModal Component, RecipeCard Component (+1 more)

### Community 13 - "Inventory Use Case"
Cohesion: 0.43
Nodes (8): AddInventoryItemRequest, AddInventoryItemUseCase.ExecuteAsync, AppDbContext, InventoryItem Entity, InventoryItemDto, Recipe Entity, RecipeIngredient Entity, ShoppingItem Entity

### Community 14 - "Dashboard Polling"
Cohesion: 0.33
Nodes (2): useDashboard(), usePollingFetch()

### Community 16 - "Model Snapshot"
Cohesion: 0.4
Nodes (3): AppDbContextModelSnapshot, CookHomie.Infrastructure.Migrations, ModelSnapshot

### Community 17 - "Repository Interface"
Cohesion: 0.5
Nodes (1): IInventoryRepository

### Community 18 - "Seed Migration"
Cohesion: 0.5
Nodes (2): CookHomie.Infrastructure.Migrations, SeedInventoryBaselineData

### Community 19 - "App DbContext"
Cohesion: 0.5
Nodes (2): DbContext, AppDbContext

### Community 20 - "DbContext Factory"
Cohesion: 0.5
Nodes (2): IDesignTimeDbContextFactory, AppDbContextFactory

### Community 21 - "Repository Tests"
Cohesion: 0.5
Nodes (1): InventoryRepositoryTests

### Community 22 - "Dev Mode Guard"
Cohesion: 0.5
Nodes (1): useDevMode()

### Community 23 - "Project Overview"
Cohesion: 0.5
Nodes (4): CookHomie, HomieOS, Local-First Web App, MCP Tools

### Community 24 - "Polling Lifecycle"
Cohesion: 0.5
Nodes (4): 30 Second Polling Lifecycle, useRecipes Composable, GetPollingData, PollingController

### Community 25 - "Add Inventory Use Case"
Cohesion: 0.67
Nodes (1): AddInventoryItemUseCase

### Community 26 - "Test DB Factory"
Cohesion: 0.67
Nodes (1): TestDbFactory

### Community 28 - "Inventory Composable"
Cohesion: 0.67
Nodes (1): useInventory()

### Community 30 - "Ingredient Matching"
Cohesion: 0.67
Nodes (1): matchIngredientStock()

### Community 31 - "Inventory Entity"
Cohesion: 1.0
Nodes (1): InventoryItem

### Community 32 - "Recipe Entity"
Cohesion: 1.0
Nodes (1): Recipe

### Community 33 - "Recipe Ingredient"
Cohesion: 1.0
Nodes (1): RecipeIngredient

### Community 34 - "Shopping Entity"
Cohesion: 1.0
Nodes (1): ShoppingItem

### Community 35 - "Add Inventory DTO"
Cohesion: 1.0
Nodes (1): AddInventoryItemRequest

### Community 36 - "Inventory DTO"
Cohesion: 1.0
Nodes (1): InventoryItemDto

### Community 37 - "Program Startup"
Cohesion: 1.0
Nodes (1): Program

### Community 39 - "Environment Types"
Cohesion: 1.0
Nodes (1): AppEnv

### Community 41 - "Recipe Detail"
Cohesion: 1.0
Nodes (2): Exact Normalized Stock Matching, useRecipeDetail Composable

### Community 42 - "Shopping List"
Cohesion: 1.0
Nodes (2): /api/shopping, useShoppingList

### Community 62 - "Frontend Milestone"
Cohesion: 1.0
Nodes (1): Milestone 2 Frontend

### Community 63 - "Hello Controller"
Cohesion: 1.0
Nodes (1): HelloController

### Community 64 - "Recipe Detail Route"
Cohesion: 1.0
Nodes (1): recipes/[id].vue

### Community 65 - "Recipe Index Route"
Cohesion: 1.0
Nodes (1): recipes/index.vue

## Knowledge Gaps
- **59 isolated node(s):** `InventoryItem`, `Recipe`, `RecipeIngredient`, `ShoppingItem`, `AddInventoryItemRequest` (+54 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **Thin community `Dashboard Polling`** (6 nodes): `useDashboard()`, `usePollingFetch()`, `useDashboard.ts`, `usePollingFetch.ts`, `dashboard.spec.ts`, `usePollingFetch.spec.ts`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Repository Interface`** (4 nodes): `IInventoryRepository.cs`, `IInventoryRepository`, `.AddAsync()`, `.GetAllAsync()`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Seed Migration`** (4 nodes): `20260426193535_SeedInventoryBaselineData.Designer.cs`, `CookHomie.Infrastructure.Migrations`, `SeedInventoryBaselineData`, `.BuildTargetModel()`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `App DbContext`** (4 nodes): `AppDbContext.cs`, `DbContext`, `AppDbContext`, `.OnModelCreating()`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `DbContext Factory`** (4 nodes): `AppDbContextFactory.cs`, `IDesignTimeDbContextFactory`, `AppDbContextFactory`, `.CreateDbContext()`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Repository Tests`** (4 nodes): `InventoryRepositoryTests.cs`, `InventoryRepositoryTests`, `.AddAsync_PersistsInventoryItem()`, `.EnsureCreated_SeedsTwoItemsPerCategory()`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Dev Mode Guard`** (4 nodes): `useDevMode()`, `app.vue`, `DevModeGuard.vue`, `useDevMode.ts`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Add Inventory Use Case`** (3 nodes): `AddInventoryItemUseCase.cs`, `AddInventoryItemUseCase`, `.ExecuteAsync()`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Test DB Factory`** (3 nodes): `TestDbFactory.cs`, `TestDbFactory`, `.CreateContext()`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Inventory Composable`** (3 nodes): `useInventory()`, `useInventory.ts`, `useInventory.spec.ts`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Ingredient Matching`** (3 nodes): `ingredient-stock.spec.ts`, `ingredientStock.ts`, `matchIngredientStock()`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Inventory Entity`** (2 nodes): `InventoryItem.cs`, `InventoryItem`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Recipe Entity`** (2 nodes): `Recipe.cs`, `Recipe`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Recipe Ingredient`** (2 nodes): `RecipeIngredient.cs`, `RecipeIngredient`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Shopping Entity`** (2 nodes): `ShoppingItem.cs`, `ShoppingItem`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Add Inventory DTO`** (2 nodes): `AddInventoryItemRequest.cs`, `AddInventoryItemRequest`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Inventory DTO`** (2 nodes): `InventoryItemDto.cs`, `InventoryItemDto`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Program Startup`** (2 nodes): `Program.cs`, `Program`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Environment Types`** (2 nodes): `env.ts`, `AppEnv`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Recipe Detail`** (2 nodes): `Exact Normalized Stock Matching`, `useRecipeDetail Composable`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Shopping List`** (2 nodes): `/api/shopping`, `useShoppingList`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Frontend Milestone`** (1 nodes): `Milestone 2 Frontend`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Hello Controller`** (1 nodes): `HelloController`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Recipe Detail Route`** (1 nodes): `recipes/[id].vue`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.
- **Thin community `Recipe Index Route`** (1 nodes): `recipes/index.vue`
  Too small to be a meaningful cluster - may be noise or needs more connections extracted.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ApiClient` connect `API Client Config` to `MCP API Tools`?**
  _High betweenness centrality (0.005) - this node is a cross-community bridge._
- **Are the 6 inferred relationships involving `get_inventory()` (e.g. with `api_inventory()` and `test_get_inventory_returns_items_dict()`) actually correct?**
  _`get_inventory()` has 6 INFERRED edges - model-reasoned connections that need verification._
- **What connects `InventoryItem`, `Recipe`, `RecipeIngredient` to the rest of the system?**
  _59 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Nuxt API Routes` be split into smaller, more focused modules?**
  _Cohesion score 0.1 - nodes in this community are weakly interconnected._
- **Should `Inventory Persistence` be split into smaller, more focused modules?**
  _Cohesion score 0.1 - nodes in this community are weakly interconnected._
- **Should `Recipe Store` be split into smaller, more focused modules?**
  _Cohesion score 0.11 - nodes in this community are weakly interconnected._
- **Should `MCP API Tools` be split into smaller, more focused modules?**
  _Cohesion score 0.13 - nodes in this community are weakly interconnected._