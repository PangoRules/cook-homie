---
title: CookHomie
status: active
start: 2026-04-25
tags: [project, homie-os, cookhomie]
---

> **Navigation:** 00 Overview → [[01-Architecture]] → [[02-DataModel]] → [[03-RepoStructure]] → [[04-Roadmap]]

CookHomie is the food and kitchen module of [[Notes/Ideas/HomieOS|HomieOS]].

A kitchen-aware AI assistant that knows what you have, what you like, what's expiring, and what you can cook next. Not a generic recipe app — a personal kitchen intelligence layer.

## 🎯 Objective

Build a local-first web app that tracks kitchen inventory, suggests recipes, generates shopping lists, and exposes all of this as MCP tools so any AI client can interact with your kitchen data.

## 🗂️ Outcome

A fully functional v1 with:
- Inventory management (fridge, freezer, pantry, spices)
- Recipe browser with ingredient match %
- Shopping list with auto-add from recipes
- Python MCP server exposing kitchen tools to AI clients
- Everything running locally via Docker Compose

## 🧩 Stack

| Layer | Tech |
|-------|------|
| Frontend | Nuxt 3 · Vue 3 |
| Backend | ASP.NET Core 10 · Clean Architecture |
| Database | PostgreSQL · EF Core |
| AI Gateway | Python MCP Server (FastMCP) |
| Infra | Docker Compose |

## Current Implementation State

- **Inventory** — fully wired end-to-end. C# Domain/Application/Infrastructure layers with EF Core migrations, `AddInventoryItemUseCase`, `InventoryController` with RFC7807 validation errors. Nuxt inventory page with `useInventory` composable, `AddItemModal`, skeleton loading, stale polling, error banner.
- **Recipes** — recipe list page (`/recipes`) and detail page (`/recipes/[id]`) with `RecipeCard`, `AddRecipeModal`, and client-side in-stock badges via `useRecipeDetail` + `ingredientStock.ts`. Recipe detail supports inline section editing, shared paginated ingredient table, and per-row "Add to shopping list" action. Nuxt server uses in-memory mock store; no C# RecipesController yet.
- **Shopping list** — `useShoppingList` composable backed by full Nuxt server API routes (`server/api/shopping/*`) and C# ShoppingController. Full CRUD + bulk add + dedup guards implemented.
- **Dashboard** — `DashboardPanel` components render actual expiring items and recipe suggestions. `InventoryItemDetailModal` provides view/edit for inventory and expiring-context actions. Missing-ingredient expansion + bulk add CTA wired from dashboard.
- **Development page** — polling playground with `PollingController` (C#) for simulated data. Dev-mode status endpoint.
- **MCP server** — `get_inventory` fully functional (calls C# API). `get_recipes` and `get_shopping_list` are stubs returning empty data.
- **Styling** — Tailwind v4 CSS-first. All styles via `assets/css/main.css` (`@layer components` classes: `.btn`, `.input`, `.card`, `.skeleton`, etc.) or inline Tailwind utilities. **No scoped `<style>` blocks** in any Vue file.
- **DRY enforcement** — reusable multi-element patterns extracted into `components/shared/`. Shared composables under `composables/`.

## 🧩 Tasks

- [x] Scaffold monorepo structure
- [x] Set up Docker Compose (PostgreSQL + API + Web + MCP)
- [x] Build C# Domain + Application layers
- [x] Build C# Infrastructure (EF Core + migrations)
- [x] Build C# WebApi inventory controller and health/OpenAPI endpoints
- [x] Build Nuxt frontend beyond inventory skeletons
- [ ] Build full Python MCP server toolset
- [x] End-to-end add-item smoke test and CI workflow
- [ ] End-to-end test: suggest recipe → build shopping list

## 🔗 Related

- [[Notes/Ideas/CookHomie|CookHomie Idea]]
- [[Notes/Ideas/HomieOS|HomieOS]]
- [[01-Architecture]]
- [[02-DataModel]]
- [[03-RepoStructure]]
- [[04-Roadmap]]
