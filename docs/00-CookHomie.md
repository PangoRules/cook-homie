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

- API, Web, and MCP projects are scaffolded under `src/`.
- The default Docker API runtime is `CookHomie.WebApi`; the spike API remains only as historical validation code.
- Inventory domain, EF Core persistence, WebApi inventory endpoints, Nuxt inventory page/proxy routes, and MCP inventory tooling exist.
- Recipes, shopping, and richer AI-assisted tools are still mostly skeletons.
- End-to-end add-item verification and CI workflow are still pending.

## 🧩 Tasks

- [x] Scaffold monorepo structure
- [x] Set up Docker Compose (PostgreSQL + API + Web + MCP)
- [x] Build C# Domain + Application layers
- [x] Build C# Infrastructure (EF Core + migrations)
- [x] Build C# WebApi inventory controller and health/OpenAPI endpoints
- [ ] Build Nuxt frontend beyond inventory skeletons
- [ ] Build full Python MCP server toolset
- [ ] End-to-end test: add item → suggest recipe → build shopping list

## 🔗 Related

- [[Notes/Ideas/CookHomie|CookHomie Idea]]
- [[Notes/Ideas/HomieOS|HomieOS]]
- [[01-Architecture]]
- [[02-DataModel]]
- [[03-RepoStructure]]
- [[04-Roadmap]]
