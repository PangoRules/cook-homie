---
title: CookHomie Roadmap
tags: [project, cookhomie, roadmap]
---

> **Navigation:** [[00-CookHomie|Overview]] → [[01-Architecture]] → [[02-DataModel]] → [[03-RepoStructure]] → 04 Roadmap

## Version 1 — MVP (Current)

**Goal:** Working local web app with inventory, recipes, shopping list, and MCP tools.

### Milestone 1 — Foundation
- [x] Create monorepo folder structure
- [x] Set up Docker Compose (PostgreSQL + API + Web + MCP)
- [x] C# Domain layer (entities, enums, interfaces)
- [x] C# Application layer (inventory use case and DTOs)
- [x] C# Infrastructure (EF Core DbContext, repository, migrations)
- [x] C# WebApi scaffold (controllers, DI wiring, Program.cs)
- [x] Tighten WebApi inventory POST to consistently use the application use case and return the intended add-item contract

### Milestone 2 — Frontend
- [x] Nuxt 3 scaffold with default layout and route skeletons
- [x] Dashboard page (expiring items, recipe matches, shopping count)
- [x] DashboardStatCard and DashboardPanel components
- [x] useDashboard composable with 30s polling
- [x] Inventory page + AddItemModal
- [ ] Recipes page + RecipeCard + AddRecipeModal
- [ ] Recipe detail page (ingredients with in-stock highlighting)
- [x] Shopping list page skeleton

### Milestone 3 — MCP Server
- [x] FastMCP scaffold + api_client.py
- [x] get_inventory tool
- [ ] get_expiring_items tool
- [ ] suggest_recipes tool
- [ ] get_missing_ingredients tool
- [ ] build_shopping_list tool
- [ ] add_to_shopping_list tool
- [x] system.md prompt

### Milestone 4 — Integration
- [ ] End-to-end: add item → suggest recipe → build shopping list
- [x] Add focused add-item E2E smoke script
- [x] Add CI workflow for API/Web/MCP checks
- [ ] Wire MCP server into Claude Code / opencode
- [ ] Replace `docker-compose.dev.yml` placeholder commands with useful hot reload workflows

---

## Version 2 — Intelligence

- Expiration tracking with alerts
- Priority-to-use items (use what expires first)
- Weekly meal planning
- Food preferences and dislikes
- Leftover suggestions
- API key auth for multi-device access

---

## Version 3 — Automation

- Barcode scanning or easier entry
- AI-assisted parsing from grocery receipts
- Budget-aware planning
- Nutrition-aware planning
- Tighter integration with other HomieOS modules (GymHomie → protein goals)

---

## 🔗 Related

- [[00-CookHomie|Overview]]
- [[Notes/Ideas/HomieOS|HomieOS]]
