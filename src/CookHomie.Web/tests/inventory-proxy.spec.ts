import { beforeEach, describe, expect, it, vi } from "vitest";

describe("inventory proxy routes", () => {
  beforeEach(() => {
    vi.resetModules();
    vi.unstubAllGlobals();
  });

  it("GET proxy forwards to /api/inventory on upstream API", async () => {
    const payload = [{ id: "1", name: "Milk" }];
    const fetchSpy = vi.fn().mockResolvedValue(payload);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("$fetch", fetchSpy);

    const { default: handler } = await import("../server/api/inventory/index.get");

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    await expect(handler({} as any)).resolves.toEqual(payload);
    expect(fetchSpy).toHaveBeenCalledWith("/api/inventory", {
      baseURL: "http://api:5000",
    });
  });

  it("POST proxy forwards body to /api/inventory on upstream API", async () => {
    const body = { name: "Eggs", quantity: 12 };
    const payload = { id: "2", ...body };
    const fetchSpy = vi.fn().mockResolvedValue(payload);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("readBody", vi.fn().mockResolvedValue(body));
    vi.stubGlobal("$fetch", fetchSpy);

    const { default: handler } = await import("../server/api/inventory/index.post");

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    await expect(handler({} as any)).resolves.toEqual(payload);
    expect(fetchSpy).toHaveBeenCalledWith("/api/inventory", {
      baseURL: "http://api:5000",
      method: "POST",
      body,
    });
  });
});
