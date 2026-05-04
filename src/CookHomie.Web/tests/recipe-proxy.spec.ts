import { beforeEach, describe, expect, it, vi } from "vitest";
import { mockEvent } from "./helpers/events";

describe("recipe proxy routes", () => {
  beforeEach(() => {
    vi.resetModules();
    vi.unstubAllGlobals();
  });

  it("GET /recipes proxy forwards to /api/recipes on upstream API", async () => {
    const payload = [
      {
        id: "r1",
        name: "Pancakes",
        instructions: "Mix and cook",
        prepMinutes: 5,
        cookMinutes: 10,
        tags: [],
        ingredients: [],
      },
    ];
    const fetchSpy = vi.fn().mockResolvedValue(payload);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("$fetch", fetchSpy);

    const { default: handler } = await import("../server/api/recipes/index.get");

    await expect(handler(mockEvent)).resolves.toEqual(payload);
    expect(fetchSpy).toHaveBeenCalledWith("/api/recipes", {
      baseURL: "http://api:5000",
    });
  });

  it("POST /recipes proxy forwards body to /api/recipes on upstream API", async () => {
    const body = {
      name: "Avocado Toast",
      instructions: "Toast and mash",
      prepMinutes: 3,
      cookMinutes: 2,
      tags: ["breakfast"],
      ingredients: [],
    };
    const payload = { id: "r2", ...body };
    const fetchSpy = vi.fn().mockResolvedValue(payload);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("readBody", vi.fn().mockResolvedValue(body));
    vi.stubGlobal("$fetch", fetchSpy);

    const { default: handler } = await import("../server/api/recipes/index.post");

    await expect(handler(mockEvent)).resolves.toEqual(payload);
    expect(fetchSpy).toHaveBeenCalledWith("/api/recipes", {
      baseURL: "http://api:5000",
      method: "POST",
      body,
    });
  });

  it("GET /recipes/:id proxy forwards to /api/recipes/:id on upstream API", async () => {
    const payload = {
      id: "r1",
      name: "Pancakes",
      instructions: "Mix and cook",
      prepMinutes: 5,
      cookMinutes: 10,
      tags: [],
      ingredients: [],
    };
    const fetchSpy = vi.fn().mockResolvedValue(payload);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("getRouterParam", (_event: unknown, _name: string) => "r1");
    vi.stubGlobal("$fetch", fetchSpy);

    const { default: handler } = await import("../server/api/recipes/[id].get");

    await expect(
      handler({ context: { params: { id: "r1" } } } as unknown as typeof mockEvent)
    ).resolves.toEqual(payload);
    expect(fetchSpy).toHaveBeenCalledWith("/api/recipes/r1", {
      baseURL: "http://api:5000",
    });
  });

  it("PATCH /recipes/:id proxy forwards body to /api/recipes/:id on upstream API", async () => {
    const body = { name: "Updated Pancakes", prepMinutes: 10 };
    const payload = {
      id: "r1",
      ...body,
      instructions: "Mix and cook",
      cookMinutes: 10,
      tags: [],
      ingredients: [],
    };
    const fetchSpy = vi.fn().mockResolvedValue(payload);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("getRouterParam", (_event: unknown, _name: string) => "r1");
    vi.stubGlobal("readBody", vi.fn().mockResolvedValue(body));
    vi.stubGlobal("$fetch", fetchSpy);

    const { default: handler } = await import("../server/api/recipes/[id].patch");

    await expect(
      handler({ context: { params: { id: "r1" } } } as unknown as typeof mockEvent)
    ).resolves.toEqual(payload);
    expect(fetchSpy).toHaveBeenCalledWith("/api/recipes/r1", {
      baseURL: "http://api:5000",
      method: "PATCH",
      body,
    });
  });

  it("GET /recipes/:id/missing proxy forwards to /api/recipes/:id/missing on upstream API", async () => {
    const payload = ["flour", "eggs"];
    const fetchSpy = vi.fn().mockResolvedValue(payload);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("getRouterParam", (_event: unknown, _name: string) => "r1");
    vi.stubGlobal("$fetch", fetchSpy);

    const { default: handler } = await import("../server/api/recipes/[id]/missing.get");

    await expect(
      handler({ context: { params: { id: "r1" } } } as unknown as typeof mockEvent)
    ).resolves.toEqual(payload);
    expect(fetchSpy).toHaveBeenCalledWith("/api/recipes/r1/missing", {
      baseURL: "http://api:5000",
    });
  });
});
