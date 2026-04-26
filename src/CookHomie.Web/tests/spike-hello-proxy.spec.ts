import { beforeEach, describe, expect, it, vi } from "vitest";

describe("spike hello proxy route", () => {
  beforeEach(() => {
    vi.resetModules();
    vi.unstubAllGlobals();
  });

  it("returns upstream payload on success", async () => {
    const payload = { message: "Hello from API" };
    const fetchSpy = vi.fn().mockResolvedValue(payload);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("$fetch", fetchSpy);
    vi.stubGlobal("createError", vi.fn());

    const { default: handler } = await import("../server/api/spike/hello.get");

    await expect(handler()).resolves.toEqual(payload);
    expect(fetchSpy).toHaveBeenCalledWith("/spike/hello", {
      baseURL: "http://api:5000"
    });
  });

  it("returns stable 502 when upstream fails", async () => {
    const fetchSpy = vi.fn().mockRejectedValue(new Error("ECONNREFUSED details"));
    const createErrorSpy = vi.fn((details: unknown) => details);

    vi.stubGlobal("defineEventHandler", (handler: unknown) => handler);
    vi.stubGlobal("useRuntimeConfig", () => ({ apiBaseUrl: "http://api:5000" }));
    vi.stubGlobal("$fetch", fetchSpy);
    vi.stubGlobal("createError", createErrorSpy);

    const { default: handler } = await import("../server/api/spike/hello.get");

    await expect(handler()).rejects.toEqual({
      statusCode: 502,
      statusMessage: "Bad Gateway",
      message: "Unable to reach upstream API"
    });
    expect(createErrorSpy).toHaveBeenCalledWith({
      statusCode: 502,
      statusMessage: "Bad Gateway",
      message: "Unable to reach upstream API"
    });
  });
});
