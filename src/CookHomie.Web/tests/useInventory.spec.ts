// src/CookHomie.Web/tests/useInventory.spec.ts
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { useInventory } from "../composables/useInventory";

describe("useInventory", () => {
  beforeEach(() => {
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));
  });

  it("returns all inventories", () => {
    const { items } = useInventory();
    expect(items.value).toEqual([]);
  });

  it("starts polling on start() and stops on stop()", async () => {
    vi.useFakeTimers();
    const fetchSpy = vi
      .fn()
      .mockResolvedValue([
        {
          id: "1",
          name: "Milk",
          category: "dairy",
          location: "Fridge",
          quantity: 1,
          unit: "liter",
          isOpened: false,
        },
      ]);
    vi.stubGlobal("$fetch", fetchSpy);
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

    const { startPolling, stopPolling } = useInventory();
    await startPolling();

    fetchSpy.mockClear();
    await vi.advanceTimersByTimeAsync(30000);
    expect(fetchSpy).toHaveBeenCalled();
    stopPolling();

    fetchSpy.mockClear();
    await vi.advanceTimersByTimeAsync(35000);
    expect(fetchSpy).not.toHaveBeenCalled();
    vi.useRealTimers();
  });

  it("refresh() forces a manual fetch", async () => {
    const fetchSpy = vi
      .fn()
      .mockResolvedValue([
        {
          id: "2",
          name: "Eggs",
          category: "dairy",
          location: "Fridge",
          quantity: 12,
          unit: "units",
          isOpened: false,
        },
      ]);
    vi.stubGlobal("$fetch", fetchSpy);
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

    const { refresh } = useInventory();
    await refresh();
    expect(fetchSpy).toHaveBeenCalledTimes(1);
  });
});
