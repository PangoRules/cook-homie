// src/CookHomie.Web/tests/usePollingFetch.spec.ts
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { usePollingFetch } from "../composables/usePollingFetch";

describe("usePollingFetch", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));
    vi.stubGlobal("useNuxtApp", () => ({}));
  });

  it("fetches immediately on mount", async () => {
    const spy = vi.fn().mockResolvedValue({ data: "test" });
    vi.stubGlobal("$fetch", spy);

    const { data, start } = usePollingFetch("/api/test");
    await start();

    expect(spy).toHaveBeenCalledWith("/api/test");
    expect(data.value).toEqual({ data: "test" });
  });

  it("polls every 30 seconds", async () => {
    const spy = vi.fn().mockResolvedValue({ data: "value" });
    vi.stubGlobal("$fetch", spy);

    const { start } = usePollingFetch("/api/test");
    await start();

    spy.mockClear();
    await vi.advanceTimersByTimeAsync(30000);

    expect(spy).toHaveBeenCalledTimes(1);
  });

  it("stops polling on stop()", async () => {
    const spy = vi.fn().mockResolvedValue({ data: "value" });
    vi.stubGlobal("$fetch", spy);

    const { start, stop } = usePollingFetch("/api/test");
    await start();
    stop();

    spy.mockClear();
    await vi.advanceTimersByTimeAsync(35000);

    expect(spy).not.toHaveBeenCalled();
  });

  it("handles fetch errors and sets error state", async () => {
    const spy = vi.fn().mockRejectedValue(new Error("network error"));
    vi.stubGlobal("$fetch", spy);
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

    const { error, start } = usePollingFetch("/api/test");
    await start();

    expect(error.value).toBe("network error");
  });
});