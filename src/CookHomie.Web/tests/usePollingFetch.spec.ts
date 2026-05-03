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

  it("pauses polling when page is hidden and resumes when visible", async () => {
    const spy = vi.fn().mockResolvedValue({ data: "value" });
    vi.stubGlobal("$fetch", spy);
    
    // Mock document.visibilityState
    const visibilityChangeListeners: Array<() => void> = [];
    vi.stubGlobal("document", {
      visibilityState: "visible",
      addEventListener: (event: string, listener: () => void) => {
        if (event === 'visibilitychange') {
          visibilityChangeListeners.push(listener);
        }
      },
      removeEventListener: () => {}
    });
    
    const { start } = usePollingFetch("/api/test");
    await start();
    
    // Advance time to make sure polling occurs
    spy.mockClear();
    await vi.advanceTimersByTimeAsync(30000);
    
    // Should have been called once
    expect(spy).toHaveBeenCalledTimes(1);
    
    // Simulate page becoming hidden by calling the visibility change listeners
    (document as any).visibilityState = "hidden";
    for (const listener of visibilityChangeListeners) {
      listener();
    }
    
    // Advance time again, polling should not occur while hidden
    spy.mockClear();
    await vi.advanceTimersByTimeAsync(30000);
    
    // Should not have been called since page was hidden
    expect(spy).toHaveBeenCalledTimes(0);
    
    // Simulate page becoming visible
    (document as any).visibilityState = "visible";
    for (const listener of visibilityChangeListeners) {
      listener();
    }
    
    // Advance time again, polling should resume
    spy.mockClear();
    await vi.advanceTimersByTimeAsync(30000);
    
    // Should have been called again since page became visible
    expect(spy).toHaveBeenCalledTimes(1);
  });
});
