import { beforeEach, describe, expect, it, vi } from "vitest";

describe("hello proxy route", () => {
  beforeEach(() => {
    vi.resetModules();
    vi.unstubAllGlobals();
  });

  it("GET proxy forwards to /api/hello on upstream API", async () => {
    const fetchSpy = vi.fn().mockResolvedValue({ message: "hello" });
    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api.example" }));
    vi.stubGlobal("$fetch", fetchSpy);

    const { default: handler } = await import("../server/api/hello.get");
    const result = await handler({} as never);

    expect(result).toEqual({ message: "hello" });
    expect(fetchSpy).toHaveBeenCalledWith("/api/hello", {
      baseURL: "http://api.example",
      method: "GET",
      headers: {
        Accept: "application/json",
      },
    });
  });
});
