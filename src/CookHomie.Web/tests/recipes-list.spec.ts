import { describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { useRecipes } from "../composables/useRecipes";

describe("useRecipes", () => {
  it("returns recipes from usePollingFetch", () => {
    const mockRecipes = [{ id: "r1", name: "Pancakes", instructions: "Mix.", prepMinutes: 5, cookMinutes: 10, tags: [], ingredients: [] }];
    vi.stubGlobal("usePollingFetch", () => ({
      data: ref(mockRecipes),
      loading: ref(false),
      error: ref(null),
      isStale: ref(false),
      start: vi.fn(),
      stop: vi.fn(),
      refresh: vi.fn()
    }));

    const { recipes, start, stop, refresh } = useRecipes();
    expect(recipes.value).toEqual(mockRecipes);
    expect(start).toBeDefined();
    expect(stop).toBeDefined();
    expect(refresh).toBeDefined();
  });

  it("delegates polling control to usePollingFetch", () => {
    const startFn = vi.fn();
    const stopFn = vi.fn();
    const refreshFn = vi.fn();
    vi.stubGlobal("usePollingFetch", () => ({
      data: ref([]),
      loading: ref(false),
      error: ref(null),
      isStale: ref(false),
      start: startFn,
      stop: stopFn,
      refresh: refreshFn
    }));

    const { start, stop, refresh } = useRecipes();
    start();
    expect(startFn).toHaveBeenCalled();
    stop();
    expect(stopFn).toHaveBeenCalled();
    refresh();
    expect(refreshFn).toHaveBeenCalled();
  });
});