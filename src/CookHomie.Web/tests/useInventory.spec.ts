// src/CookHomie.Web/tests/useInventory.spec.ts
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { useInventory } from "../composables/useInventory";

describe("useInventory", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));
    vi.stubGlobal("useNuxtApp", () => ({}));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("returns inventory items from usePollingFetch", () => {
    const mockItems = [{ id: "1", name: "Milk", category: "dairy", location: "Fridge", quantity: 1, unit: "liter", isOpened: false }];
    vi.stubGlobal("usePollingFetch", () => ({
      data: ref(mockItems),
      loading: ref(false),
      error: ref(null),
      isStale: ref(false),
      start: vi.fn(),
      stop: vi.fn(),
      refresh: vi.fn()
    }));

    const { items, startPolling, stopPolling, refresh, addInventoryItem } = useInventory();
    expect(items.value).toEqual(mockItems);
    expect(startPolling).toBeDefined();
    expect(stopPolling).toBeDefined();
    expect(refresh).toBeDefined();
    expect(addInventoryItem).toBeDefined();
  });

  it("delegates polling start/stop to usePollingFetch", () => {
    const startFn = vi.fn();
    const stopFn = vi.fn();
    vi.stubGlobal("usePollingFetch", () => ({
      data: ref([]),
      loading: ref(false),
      error: ref(null),
      isStale: ref(false),
      start: startFn,
      stop: stopFn,
      refresh: vi.fn()
    }));

    const { startPolling, stopPolling } = useInventory();
    startPolling();
    expect(startFn).toHaveBeenCalled();
    stopPolling();
    expect(stopFn).toHaveBeenCalled();
  });

  it("delegates refresh to usePollingFetch.refresh", async () => {
    const refreshFn = vi.fn().mockResolvedValue([]);
    vi.stubGlobal("usePollingFetch", () => ({
      data: ref([]),
      loading: ref(false),
      error: ref(null),
      isStale: ref(false),
      start: vi.fn(),
      stop: vi.fn(),
      refresh: refreshFn
    }));

    const { refresh } = useInventory();
    await refresh();
    expect(refreshFn).toHaveBeenCalled();
  });
});
