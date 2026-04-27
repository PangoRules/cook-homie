# CookHomie v1 Scaffolding + Add Item Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the full CookHomie v1 foundation with a validated spike, production-ready scaffolding for API/Web/MCP, and one working end-to-end feature (`add inventory item`).

**Architecture:** Execute in three phases: (1) short spike proving service networking, (2) full scaffolding with clean boundaries, (3) first vertical slice (`add item`) implemented via TDD. Keep services decoupled through HTTP contracts and enforce C# Clean Architecture via project references.

**Tech Stack:** ASP.NET Core 10 + EF Core + PostgreSQL 16, Nuxt 3 + Vue 3 + Vitest, Python 3.11+ + FastMCP + httpx, Docker Compose.

**Current project state (2026-04-26):** Tasks 1-10 are scaffolded. The repository also contains partial later-task work: `CookHomie.WebApi` is already the default API Docker runtime, `GET/POST /api/inventory` exist in WebApi, Nuxt inventory proxy GET/POST routes exist, and MCP `get_inventory` calls the API. Remaining work should tighten the add-item vertical slice contract, add missing UI/modal pieces, add E2E/CI verification, and make spike validation optional or historical.

---

## File Map (Create/Modify Responsibilities)

- `src/CookHomie.Api/` — C# solution root and layer projects.
- `src/CookHomie.Api/CookHomie.Domain/` — entities/enums/repository interfaces only.
- `src/CookHomie.Api/CookHomie.Application/` — DTOs and use cases.
- `src/CookHomie.Api/CookHomie.Infrastructure/` — EF Core DbContext, repositories, migrations.
- `src/CookHomie.Api/CookHomie.WebApi/` — controllers, DI registration, app startup.
- `src/CookHomie.Web/` — Nuxt app, pages/components/composables, proxy routes.
- `src/CookHomie.MCP/` — FastMCP server, API client, tool modules.
- `docker/` — Dockerfiles for API/Web/MCP.
- `docker-compose.yml` + `docker-compose.dev.yml` — prod-like + dev orchestration.
- `.env.example` — baseline environment variables.
- `.github/workflows/ci.yml` — build/type/lint smoke CI.
- `README.md` + `CONTRIBUTING.md` — bootstrap and contributor workflow.
- `docs/superpowers/specs/2026-04-25-v1-scaffolding-design.md` — approved design baseline.

---

### Task 1: Bootstrap Monorepo Skeleton

**Files:**
- Create: `README.md`
- Create: `CONTRIBUTING.md`
- Create: `.editorconfig`
- Create: `.env.example`
- Modify: `.gitignore`

- [ ] **Step 1: Write the failing documentation existence check**

```bash
test -f README.md && test -f CONTRIBUTING.md && test -f .env.example
```

- [ ] **Step 2: Run check to verify it fails**

Run: `test -f README.md && test -f CONTRIBUTING.md && test -f .env.example`
Expected: non-zero exit code because files do not exist yet

- [ ] **Step 3: Create root docs and config files**

```md
# README.md
# CookHomie

Local-first kitchen intelligence app (API + Web + MCP).

## Quick Start
1. Copy env: `cp .env.example .env`
2. Start stack: `docker compose -f docker-compose.dev.yml up --build`
3. Open web: `http://localhost:3000`

## Services
- Web: 3000
- API: 5000
- MCP: 8000
- Postgres: 5432
```

```env
# .env.example
POSTGRES_USER=cookhomie
POSTGRES_PASSWORD=dev_password
POSTGRES_DB=cookhomie
POSTGRES_PORT=5432

API_PORT=5000
WEB_PORT=3000
MCP_PORT=8000

ASPNETCORE_ENVIRONMENT=Development
API_BASE_URL=http://api:5000
```

- [ ] **Step 4: Update `.gitignore` for Node + Python artifacts**

```gitignore
# Node
node_modules/
.nuxt/
.output/

# Python
__pycache__/
*.pyc
.venv/

# Env
.env
```

- [ ] **Step 5: Re-run existence check**

Run: `test -f README.md && test -f CONTRIBUTING.md && test -f .env.example`
Expected: exit code 0

- [ ] **Step 6: Commit**

```bash
git add README.md CONTRIBUTING.md .editorconfig .env.example .gitignore
git commit -m "chore: bootstrap monorepo root docs and config"
```

---

### Task 2: Add Docker Foundation (Prod + Dev)

**Files:**
- Create: `docker/Dockerfile.api`
- Create: `docker/Dockerfile.web`
- Create: `docker/Dockerfile.mcp`
- Create: `docker-compose.yml`
- Create: `docker-compose.dev.yml`
- Create: `.dockerignore`

- [ ] **Step 1: Write failing compose config check**

```bash
docker compose -f docker-compose.yml config
```

- [ ] **Step 2: Run check to verify it fails**

Run: `docker compose -f docker-compose.yml config`
Expected: error that `docker-compose.yml` does not exist

- [ ] **Step 3: Create Dockerfiles and compose definitions**

```yaml
# docker-compose.yml
services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      POSTGRES_DB: ${POSTGRES_DB}
    ports:
      - "${POSTGRES_PORT}:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER}"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build:
      context: .
      dockerfile: docker/Dockerfile.api
    ports:
      - "${API_PORT}:5000"
    depends_on:
      postgres:
        condition: service_healthy

  web:
    build:
      context: .
      dockerfile: docker/Dockerfile.web
    ports:
      - "${WEB_PORT}:3000"
    depends_on:
      - api

  mcp:
    build:
      context: .
      dockerfile: docker/Dockerfile.mcp
    ports:
      - "${MCP_PORT}:8000"
    depends_on:
      - api
```

- [ ] **Step 4: Validate compose syntax**

Run: `docker compose -f docker-compose.yml config`
Expected: rendered YAML output, no parser errors

- [ ] **Step 5: Commit**

```bash
git add docker docker-compose.yml docker-compose.dev.yml .dockerignore
git commit -m "chore: add docker compose foundation for api web mcp"
```

---

### Task 3: Build Spike API (C# Minimal Endpoint)

**Files:**
- Create: `src/CookHomie.Api/CookHomie.SpikeApi/CookHomie.SpikeApi.csproj`
- Create: `src/CookHomie.Api/CookHomie.SpikeApi/Program.cs`
- Create: `src/CookHomie.Api/CookHomie.SpikeApi/Controllers/SpikeController.cs`
- Test: `src/CookHomie.Api/CookHomie.SpikeApi.Tests/SpikeControllerTests.cs`

- [ ] **Step 1: Write failing API integration test**

```csharp
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class SpikeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public SpikeControllerTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetHello_ReturnsOk()
    {
        var response = await _client.GetAsync("/spike/hello");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test src/CookHomie.Api/CookHomie.SpikeApi.Tests`
Expected: FAIL because API project/controller does not exist yet

- [ ] **Step 3: Implement minimal API and controller**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

public partial class Program { }
```

```csharp
// Controllers/SpikeController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("spike")]
public class SpikeController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello() => Ok(new { message = "Hello from C#", timestamp = DateTime.UtcNow });
}
```

- [ ] **Step 4: Re-run tests**

Run: `dotnet test src/CookHomie.Api/CookHomie.SpikeApi.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Api
git commit -m "feat: add spike api hello endpoint with test"
```

---

### Task 4: Build Spike Web (Nuxt Page + Proxy)

**Files:**
- Create: `src/CookHomie.Web/package.json`
- Create: `src/CookHomie.Web/nuxt.config.ts`
- Create: `src/CookHomie.Web/pages/index.vue`
- Create: `src/CookHomie.Web/server/api/spike/hello.get.ts`
- Test: `src/CookHomie.Web/tests/index.spec.ts`

- [ ] **Step 1: Write failing UI test**

```ts
import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import IndexPage from "../pages/index.vue";

describe("IndexPage", () => {
  it("renders spike message heading", () => {
    const wrapper = mount(IndexPage);
    expect(wrapper.text()).toContain("CookHomie Spike");
  });
});
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/CookHomie.Web && npm test`
Expected: FAIL because project and page are missing

- [ ] **Step 3: Implement minimal Nuxt app + proxy route**

```ts
// server/api/spike/hello.get.ts
export default defineEventHandler(async () => {
  const apiBase = process.env.API_BASE_URL || "http://api:5000";
  return await $fetch(`${apiBase}/spike/hello`);
});
```

```vue
<!-- pages/index.vue -->
<template>
  <main>
    <h1>CookHomie Spike</h1>
    <p v-if="data">{{ data.message }}</p>
  </main>
</template>

<script setup lang="ts">
const { data } = await useFetch("/api/spike/hello");
</script>
```

- [ ] **Step 4: Re-run tests**

Run: `cd src/CookHomie.Web && npm test`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web
git commit -m "feat: add nuxt spike page and api proxy route"
```

---

### Task 5: Build Spike MCP (Tool Calls API)

**Files:**
- Create: `src/CookHomie.MCP/pyproject.toml`
- Create: `src/CookHomie.MCP/server.py`
- Create: `src/CookHomie.MCP/api_client.py`
- Create: `src/CookHomie.MCP/tools/spike.py`
- Test: `src/CookHomie.MCP/tests/test_spike_tool.py`

- [ ] **Step 1: Write failing MCP tool test**

```python
import pytest
from tools.spike import hello_world
from api_client import ApiClient

@pytest.mark.asyncio
async def test_hello_world_returns_message():
    async def fake_get_spike_hello(self):
        return {"message": "Hello from C#"}

    ApiClient.get_spike_hello = fake_get_spike_hello
    result = await hello_world()
    assert "message" in result
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/CookHomie.MCP && pytest tests/test_spike_tool.py -q`
Expected: FAIL because module/tool not defined

- [ ] **Step 3: Implement API client and MCP tool**

```python
# api_client.py
import httpx

class ApiClient:
    def __init__(self, base_url: str) -> None:
        self.base_url = base_url.rstrip("/")

    async def get_spike_hello(self) -> dict:
        async with httpx.AsyncClient(timeout=10.0) as client:
            response = await client.get(f"{self.base_url}/spike/hello")
            response.raise_for_status()
            return response.json()
```

```python
# tools/spike.py
from api_client import ApiClient

async def hello_world() -> dict:
    client = ApiClient(base_url="http://api:5000")
    return await client.get_spike_hello()
```

- [ ] **Step 4: Re-run tests**

Run: `cd src/CookHomie.MCP && pytest tests/test_spike_tool.py -q`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.MCP
git commit -m "feat: add mcp spike tool and api client"
```

---

### Task 6: Verify Spike End-to-End via Docker

**Files:**
- Create: `scripts/verify_spike.sh`
- Modify: `README.md`

- [ ] **Step 1: Write failing verification script**

```bash
#!/usr/bin/env bash
set -euo pipefail
curl -fsS http://localhost:5000/spike/hello >/dev/null
curl -fsS http://localhost:3000/api/spike/hello >/dev/null
echo "Spike verification passed"
```

- [ ] **Step 2: Run verification before services are up**

Run: `bash scripts/verify_spike.sh`
Expected: FAIL (connection refused)

- [ ] **Step 3: Start stack and rerun verification**

Run: `docker compose -f docker-compose.yml up --build -d && bash scripts/verify_spike.sh`
Expected: `Spike verification passed`

- [ ] **Step 4: Document spike commands in README**

```md
## Spike Validation
1. `docker compose -f docker-compose.yml up --build -d`
2. `bash scripts/verify_spike.sh`
3. `docker compose down`
```

- [ ] **Step 5: Commit**

```bash
git add scripts/verify_spike.sh README.md
git commit -m "chore: add spike e2e verification script"
```

---

### Task 7: Scaffold Clean Architecture API Projects

**Files:**
- Create: `src/CookHomie.Api/CookHomie.sln`
- Create: `src/CookHomie.Api/CookHomie.Domain/**/*`
- Create: `src/CookHomie.Api/CookHomie.Application/**/*`
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/**/*`
- Create: `src/CookHomie.Api/CookHomie.WebApi/**/*`
- Test: `src/CookHomie.Api/tests/CookHomie.Application.Tests/AddInventoryItemUseCaseTests.cs`

- [ ] **Step 1: Write failing use-case unit test**

```csharp
using Xunit;

public class AddInventoryItemUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidInput_ReturnsCreatedItem()
    {
        var useCase = TestFactory.CreateUseCase();
        var request = new AddInventoryItemRequest("Milk", "dairy", "Fridge", 1m, "liter", null, false, null);
        var result = await useCase.ExecuteAsync(request);
        Assert.Equal("Milk", result.Name);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test src/CookHomie.Api/tests/CookHomie.Application.Tests`
Expected: FAIL because projects/types are missing

- [ ] **Step 3: Create projects, domain entities, and application contracts**

```csharp
// CookHomie.Domain/Entities/InventoryItem.cs
public class InventoryItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Location Location { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateOnly? ExpiresAt { get; set; }
    public bool IsOpened { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

```csharp
// CookHomie.Application/UseCases/Inventory/AddInventoryItemUseCase.cs
public class AddInventoryItemUseCase
{
    private readonly IInventoryRepository _repo;
    public AddInventoryItemUseCase(IInventoryRepository repo) => _repo = repo;

    public async Task<InventoryItemDto> ExecuteAsync(AddInventoryItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Name is required");
        if (request.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");

        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Category = request.Category,
            Location = Enum.Parse<Location>(request.Location, ignoreCase: true),
            Quantity = request.Quantity,
            Unit = request.Unit,
            ExpiresAt = request.ExpiresAt,
            IsOpened = request.IsOpened,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(item);
        return InventoryItemDto.FromEntity(item);
    }
}
```

- [ ] **Step 4: Re-run tests**

Run: `dotnet test src/CookHomie.Api/tests/CookHomie.Application.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Api
git commit -m "feat: scaffold clean architecture api with inventory use case"
```

---

### Task 8: Add EF Core Infrastructure + Initial Migration

**Files:**
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/Persistence/AppDbContext.cs`
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/Repositories/InventoryRepository.cs`
- Create: `src/CookHomie.Api/CookHomie.Infrastructure/Migrations/*`
- Modify: `src/CookHomie.Api/CookHomie.WebApi/Program.cs`
- Test: `src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests/InventoryRepositoryTests.cs`

- [ ] **Step 1: Write failing repository integration test**

```csharp
[Fact]
public async Task AddAsync_PersistsInventoryItem()
{
    await using var db = TestDbFactory.CreateContext();
    var repo = new InventoryRepository(db);
    var item = new InventoryItem { Id = Guid.NewGuid(), Name = "Eggs", Category = "dairy", Location = Location.Fridge, Quantity = 12, Unit = "units" };
    await repo.AddAsync(item);

    var stored = await db.InventoryItems.FindAsync(item.Id);
    Assert.NotNull(stored);
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests`
Expected: FAIL because DbContext/repository are missing

- [ ] **Step 3: Implement DbContext, repository, and migration**

```csharp
public class AppDbContext : DbContext
{
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
```

```bash
dotnet ef migrations add InitialCreate --project src/CookHomie.Api/CookHomie.Infrastructure --startup-project src/CookHomie.Api/CookHomie.WebApi
```

- [ ] **Step 4: Re-run repository tests**

Run: `dotnet test src/CookHomie.Api/tests/CookHomie.Infrastructure.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Api
git commit -m "feat: add ef core infrastructure and initial migration"
```

---

### Task 9: Scaffold Nuxt Production Structure

**Files:**
- Create: `src/CookHomie.Web/layouts/default.vue`
- Create: `src/CookHomie.Web/pages/{inventory.vue,shopping.vue,recipes/index.vue,recipes/[id].vue}`
- Create: `src/CookHomie.Web/components/**`
- Create: `src/CookHomie.Web/composables/{useApi.ts,useInventory.ts,useRecipes.ts,useShopping.ts}`
- Create: `src/CookHomie.Web/types/index.ts`
- Test: `src/CookHomie.Web/tests/useInventory.spec.ts`

- [ ] **Step 1: Write failing composable test**

```ts
import { describe, it, expect, vi } from "vitest";
import { useInventory } from "../composables/useInventory";

describe("useInventory", () => {
  it("calls /api/inventory when loading items", async () => {
    globalThis.$fetch = vi.fn().mockResolvedValue([{ id: "1", name: "Milk" }]) as any;
    const { loadInventory, items } = useInventory();
    await loadInventory();
    expect(items.value.length).toBe(1);
  });
});
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/CookHomie.Web && npm test -- useInventory.spec.ts`
Expected: FAIL because composable is missing

- [ ] **Step 3: Implement composables and page skeletons**

```ts
// composables/useInventory.ts
import type { InventoryItem } from "~/types";

export const useInventory = () => {
  const items = useState<InventoryItem[]>("inventory-items", () => []);

  const loadInventory = async () => {
    items.value = await $fetch<InventoryItem[]>("/api/inventory");
  };

  return { items, loadInventory };
};
```

- [ ] **Step 4: Re-run tests**

Run: `cd src/CookHomie.Web && npm test -- useInventory.spec.ts`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web
git commit -m "feat: scaffold nuxt pages components and inventory composable"
```

---

### Task 10: Scaffold MCP Production Structure

**Files:**
- Create: `src/CookHomie.MCP/tools/inventory.py`
- Create: `src/CookHomie.MCP/tools/recipes.py`
- Create: `src/CookHomie.MCP/tools/shopping.py`
- Modify: `src/CookHomie.MCP/server.py`
- Create: `src/CookHomie.MCP/prompts/system.md`
- Test: `src/CookHomie.MCP/tests/test_inventory_tool.py`

- [ ] **Step 1: Write failing inventory tool test**

```python
import pytest
from tools.inventory import get_inventory

@pytest.mark.asyncio
async def test_get_inventory_returns_items_dict(monkeypatch):
    async def fake_get_inventory(location=None):
        return [{"id": "1", "name": "Milk"}]

    monkeypatch.setattr("tools.inventory.client.get_inventory", fake_get_inventory)
    result = await get_inventory()
    assert "items" in result
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/CookHomie.MCP && pytest tests/test_inventory_tool.py -q`
Expected: FAIL because tool module is missing

- [ ] **Step 3: Implement tool modules and registration**

```python
# tools/inventory.py
from api_client import ApiClient

client = ApiClient(base_url="http://api:5000")

async def get_inventory(location: str | None = None) -> dict:
    items = await client.get_inventory(location=location)
    return {"items": items}
```

```python
# server.py
from fastmcp import FastMCP
from tools.inventory import get_inventory

mcp = FastMCP("cookhomie")
mcp.tool()(get_inventory)
```

- [ ] **Step 4: Re-run tests**

Run: `cd src/CookHomie.MCP && pytest tests/test_inventory_tool.py -q`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.MCP
git commit -m "feat: scaffold mcp tools modules and server registration"
```

---

### Task 11: Implement API Add Item Vertical Slice

**Current status:** Partially implemented. `CookHomie.WebApi/Controllers/InventoryController.cs` already exposes `GET /api/inventory` and `POST /api/inventory`, but `POST` currently writes through `IInventoryRepository` directly and returns `200 OK`. Complete this task by routing creation through the application use case or explicitly updating the planned contract, then align tests and docs with the chosen behavior.

**Files:**
- Modify: `src/CookHomie.Api/CookHomie.Application/UseCases/Inventory/AddInventoryItemUseCase.cs`
- Modify: `src/CookHomie.Api/CookHomie.WebApi/Controllers/InventoryController.cs`
- Modify: `src/CookHomie.Api/CookHomie.Infrastructure/Repositories/InventoryRepository.cs`
- Test: `src/CookHomie.Api/tests/CookHomie.WebApi.Tests/InventoryControllerTests.cs`

- [ ] **Step 1: Write failing controller test for the intended POST /api/inventory contract**

```csharp
[Fact]
public async Task PostInventory_WithValidPayload_ReturnsCreated()
{
    var payload = new
    {
        name = "Milk",
        category = "dairy",
        location = "Fridge",
        quantity = 1.0m,
        unit = "liter"
    };

    var response = await _client.PostAsJsonAsync("/api/inventory", payload);
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test src/CookHomie.Api/tests/CookHomie.WebApi.Tests --filter PostInventory_WithValidPayload_ReturnsCreated`
Expected: FAIL in the current code because the endpoint exists but returns `200 OK` and bypasses the application use case.

- [ ] **Step 3: Implement controller + use-case wiring**

```csharp
[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly AddInventoryItemUseCase _addInventoryItem;

    public InventoryController(AddInventoryItemUseCase addInventoryItem) => _addInventoryItem = addInventoryItem;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddInventoryItemRequest request)
    {
        var item = await _addInventoryItem.ExecuteAsync(request);
        return Created($"/api/inventory/{item.Id}", item);
    }
}
```

- [ ] **Step 4: Re-run targeted and full API tests**

Run: `dotnet test src/CookHomie.Api/tests`
Expected: PASS for application/infrastructure/webapi tests

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Api
git commit -m "feat: implement api add inventory item endpoint"
```

---

### Task 12: Implement Web Add Item Flow

**Current status:** Partially implemented. `useInventory.ts` already has `addInventoryItem`, and Nuxt proxy routes exist for `GET` and `POST /api/inventory`. There is no `components/inventory/AddItemModal.vue` yet, and `pages/inventory.vue` currently renders a simple list with no add-item UI.

**Files:**
- Create: `src/CookHomie.Web/components/inventory/AddItemModal.vue`
- Modify: `src/CookHomie.Web/composables/useInventory.ts`
- Modify: `src/CookHomie.Web/pages/inventory.vue`
- Create: `src/CookHomie.Web/server/api/inventory/index.post.ts`
- Test: `src/CookHomie.Web/tests/AddItemModal.spec.ts`

- [ ] **Step 1: Write failing modal submission test**

```ts
import { mount } from "@vue/test-utils";
import { describe, it, expect, vi } from "vitest";
import AddItemModal from "../components/inventory/AddItemModal.vue";

describe("AddItemModal", () => {
  it("submits form and emits added event", async () => {
    const wrapper = mount(AddItemModal);
    await wrapper.find('input[name="name"]').setValue("Milk");
    await wrapper.find('button[type="submit"]').trigger("click");
    expect(wrapper.emitted("added")).toBeTruthy();
  });
});
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/CookHomie.Web && npm test -- AddItemModal.spec.ts`
Expected: FAIL because component/proxy not implemented

- [ ] **Step 3: Implement modal, composable add action, and proxy endpoint**

```ts
// composables/useInventory.ts
const addInventoryItem = async (payload: AddInventoryItemPayload) => {
  const created = await $fetch<InventoryItem>("/api/inventory", {
    method: "POST",
    body: payload,
  });
  items.value.unshift(created);
  return created;
};
```

```ts
// server/api/inventory/index.post.ts
export default defineEventHandler(async (event) => {
  const body = await readBody(event);
  const apiBase = process.env.API_BASE_URL || "http://api:5000";
  return await $fetch(`${apiBase}/api/inventory`, { method: "POST", body });
});
```

- [ ] **Step 4: Re-run tests**

Run: `cd src/CookHomie.Web && npm test`
Expected: PASS for modal/composable tests

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.Web
git commit -m "feat: implement inventory add item modal and proxy"
```

---

### Task 13: Implement MCP `get_inventory` Tool

**Status:** ✓ Complete (2026-04-27). Implemented `count` field in tool response.

**Current status:** Partially implemented. `api_client.py` already supports `get_inventory(location=None)`, and `tools/inventory.py` passes the optional location to the client. The current tool returns `{ "items": items }`; complete this task by deciding whether the tool contract should also include `count`, then align tests and documentation.

**Files:**
- Modify: `src/CookHomie.MCP/api_client.py`
- Modify: `src/CookHomie.MCP/tools/inventory.py`
- Modify: `src/CookHomie.MCP/server.py`
- Test: `src/CookHomie.MCP/tests/test_inventory_tool.py`

- [ ] **Step 1: Write failing tool test for location filtering**

```python
@pytest.mark.asyncio
async def test_get_inventory_passes_location(monkeypatch):
    called = {"location": None}

    async def fake_get_inventory(location=None):
      called["location"] = location
      return []

    monkeypatch.setattr("tools.inventory.client.get_inventory", fake_get_inventory)
    await get_inventory(location="Fridge")
    assert called["location"] == "Fridge"
```

- [ ] **Step 2: Run test to verify the current contract gap**

Run: `cd src/CookHomie.MCP && pytest tests/test_inventory_tool.py -q`
Expected: the location-passing test may already pass; add a failing assertion for any missing intended contract such as `count` if that remains required.

- [ ] **Step 3: Implement API client GET /api/inventory and tool wrapper**

```python
class ApiClient:
    async def get_inventory(self, location: str | None = None) -> list[dict]:
        params = {"location": location} if location else None
        async with httpx.AsyncClient(timeout=10.0) as client:
            response = await client.get(f"{self.base_url}/api/inventory", params=params)
            response.raise_for_status()
            return response.json()
```

```python
async def get_inventory(location: str | None = None) -> dict:
    items = await client.get_inventory(location=location)
    return {"items": items, "count": len(items)}
```

- [ ] **Step 4: Re-run MCP tests**

Run: `cd src/CookHomie.MCP && pytest -q`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/CookHomie.MCP
git commit -m "feat: implement mcp get_inventory tool with location filter"
```

---

### Task 14: End-to-End Verification + CI Stub + Docs Sync

**Files:**
- Create: `.github/workflows/ci.yml`
- Modify: `README.md`
- Modify: `CONTRIBUTING.md`
- Create: `scripts/verify_add_item_e2e.sh`

- [ ] **Step 1: Write failing E2E smoke script**

```bash
#!/usr/bin/env bash
set -euo pipefail

curl -fsS -X POST http://localhost:5000/api/inventory \
  -H 'content-type: application/json' \
  -d '{"name":"Milk","category":"dairy","location":"Fridge","quantity":1,"unit":"liter"}' >/tmp/add-item.json

curl -fsS http://localhost:5000/api/inventory | grep -q "Milk"
curl -fsS http://localhost:3000/api/inventory | grep -q "Milk"

echo "E2E add-item verification passed"
```

- [ ] **Step 2: Run script before stack is up**

Run: `bash scripts/verify_add_item_e2e.sh`
Expected: FAIL (connection refused)

- [ ] **Step 3: Start stack and run full verification chain**

Run: `docker compose -f docker-compose.dev.yml up --build -d && bash scripts/verify_add_item_e2e.sh`
Expected: `E2E add-item verification passed`

- [ ] **Step 4: Add CI stub for build/test/lint**

```yaml
name: CI

on:
  push:
  pull_request:

jobs:
  checks:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - run: dotnet build src/CookHomie.Api/CookHomie.sln
      - uses: actions/setup-node@v4
        with:
          node-version: 20
      - run: cd src/CookHomie.Web && npm ci && npm run test
      - uses: actions/setup-python@v5
        with:
          python-version: '3.11'
      - run: cd src/CookHomie.MCP && pip install -e . && pytest -q
```

- [ ] **Step 5: Update docs with exact runbook**

```md
## Local Development Runbook
1. `cp .env.example .env`
2. `docker compose -f docker-compose.dev.yml up --build`
3. Open `http://localhost:3000`
4. Run smoke checks:
   - `bash scripts/verify_spike.sh`
   - `bash scripts/verify_add_item_e2e.sh`
```

- [ ] **Step 6: Commit**

```bash
git add .github/workflows/ci.yml README.md CONTRIBUTING.md scripts/verify_add_item_e2e.sh
git commit -m "chore: add e2e verification script ci stub and runbook"
```

---

### Task 15: Cut Over to WebApi and Retire Spike from Default Flow

**Current status:** Partially implemented ahead of schedule. `docker/Dockerfile.api` already restores, publishes, and runs `CookHomie.WebApi.dll`, and `docker-compose.yml` supplies the WebApi connection string. The remaining work is to verify the cutover, update stale docs/scripts, and decide whether `scripts/verify_spike.sh` should skip by default or remain a historical manual check.

**Files:**
- Modify: `docker/Dockerfile.api`
- Modify: `docker-compose.yml`
- Modify: `docker-compose.dev.yml`
- Modify: `README.md`
- Modify: `scripts/verify_spike.sh`

- [ ] **Step 1: Verify runtime check proving compose points at WebApi**

Run: `grep -q 'CookHomie.WebApi.dll' docker/Dockerfile.api`
Expected: PASS because the Dockerfile already points to `CookHomie.WebApi.dll`

- [ ] **Step 2: Keep API container build/publish target on WebApi**

```dockerfile
# docker/Dockerfile.api (key lines)
COPY src/CookHomie.Api/CookHomie.WebApi/CookHomie.WebApi.csproj ./CookHomie.WebApi/
RUN dotnet restore ./CookHomie.WebApi/CookHomie.WebApi.csproj -p:TargetFramework=net10.0

COPY src/CookHomie.Api/CookHomie.WebApi/ ./CookHomie.WebApi/
COPY src/CookHomie.Api/CookHomie.Application/ ./CookHomie.Application/
COPY src/CookHomie.Api/CookHomie.Infrastructure/ ./CookHomie.Infrastructure/
COPY src/CookHomie.Api/CookHomie.Domain/ ./CookHomie.Domain/

RUN dotnet publish ./CookHomie.WebApi/CookHomie.WebApi.csproj -c Release -f net10.0 -p:TargetFramework=net10.0 --no-restore -o /app/publish
ENTRYPOINT ["dotnet", "CookHomie.WebApi.dll"]
```

- [ ] **Step 3: Update compose + docs to treat WebApi as the default API runtime**

```yaml
# docker-compose.yml (api service)
api:
  build:
    context: .
    dockerfile: docker/Dockerfile.api
  environment:
    ASPNETCORE_URLS: http://+:5000
    ASPNETCORE_ENVIRONMENT: Development
    ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=${POSTGRES_DB:-cookhomie};Username=${POSTGRES_USER:-cookhomie};Password=${POSTGRES_PASSWORD:-cookhomie_dev_password}
```

```md
## API Runtime
- Default API service is now `CookHomie.WebApi` (production path).
- `CookHomie.SpikeApi` remains historical validation code and is not part of the default Docker validation chain.
```

- [ ] **Step 4: Adjust spike verifier to be optional and non-blocking**

```bash
# scripts/verify_spike.sh
#!/usr/bin/env bash
set -euo pipefail

if [[ "${ENABLE_SPIKE_CHECK:-0}" != "1" ]]; then
  echo "Skipping spike verification (set ENABLE_SPIKE_CHECK=1 to enable)."
  exit 0
fi

curl -fsS http://localhost:5000/spike/hello >/dev/null
curl -fsS http://localhost:3000/api/spike/hello >/dev/null
echo "Spike verification passed"
```

- [ ] **Step 5: Verify cutover end-to-end on default stack**

Run: `docker compose -f docker-compose.yml up --build -d && bash scripts/verify_add_item_e2e.sh`
Expected: `E2E add-item verification passed` with no dependency on spike endpoints

- [ ] **Step 6: Verify Swagger is exposed from WebApi in local development**

Run: `curl -fsS http://localhost:5000/swagger/v1/swagger.json >/dev/null && curl -fsSI http://localhost:5000/swagger | grep -q '200\|301\|302'`
Expected: commands succeed, proving OpenAPI JSON and Swagger UI endpoint are reachable

- [ ] **Step 7: Commit**

```bash
git add docker/Dockerfile.api docker-compose.yml docker-compose.dev.yml README.md CONTRIBUTING.md docs scripts/verify_spike.sh
git commit -m "chore: cut over default api runtime from spike to webapi"
```

**Implementation note (required behavior):**
- Swagger helper routes (`/swagger`, `/swagger/index.html`) must be reachable in dev but excluded from OpenAPI operation listing.
- `/health` must validate both API readiness and database connectivity; return `200` only when both checks are healthy.
- If any health check fails, return RFC7807 Problem Details (`application/problem+json`) with machine-readable check details.
- Unhandled runtime exceptions in API endpoints must be emitted as Problem Details responses instead of raw stack/error text.

---

## Final Validation Checklist (Run After Task 15)

- [ ] `dotnet test src/CookHomie.Api/tests`
- [ ] `cd src/CookHomie.Web && npm test`
- [ ] `cd src/CookHomie.MCP && pytest -q`
- [ ] `docker compose -f docker-compose.yml up --build -d`
- [ ] `bash scripts/verify_add_item_e2e.sh`
- [ ] `curl -fsS http://localhost:5000/swagger/v1/swagger.json >/dev/null`
- [ ] `curl -fsSI http://localhost:5000/swagger >/dev/null`
- [ ] `bash scripts/verify_spike.sh` (expect skip message unless `ENABLE_SPIKE_CHECK=1`)
- [ ] `docker compose down`

Expected: all default checks succeed with `CookHomie.WebApi` as the primary API path; spike validation is optional.
