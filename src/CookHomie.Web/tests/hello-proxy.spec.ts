import { beforeEach, describe, expect, it, vi } from "vitest";
import { mockEvent } from "./helpers/events";

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
    const result = await handler(mockEvent);

    expect(result).toEqual({ message: "hello" });
    expect(fetchSpy).toHaveBeenCalledWith("/api/hello", {
      baseURL: "http://api.example",
      headers: {
        Accept: "application/json",
      },
    });
  });
});
