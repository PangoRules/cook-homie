import { describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { useRecipes } from "../composables/useRecipes";

// Mock usePollingFetch to satisfy the composables that use it
vi.stubGlobal("usePollingFetch", vi.fn().mockReturnValue({
  data: ref(null),
  loading: ref(true),
  error: ref(null),
  isStale: ref(false),
  start: vi.fn(),
  stop: vi.fn(),
  refresh: vi.fn()
}));

describe("useRecipes", () => {
  it("fetches recipes from /api/recipes", async () => {
    vi.stubGlobal("useState", (_k: string, init: () => unknown) => ref(init()));
    
    const mockRecipes = [{ id: "r1", name: "Pancakes", instructions: "Mix.", prepMinutes: 5, cookMinutes: 10, tags: [], ingredients: [] }];
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue(mockRecipes));

    const { recipes, start } = useRecipes();
    await start();

    expect(vi.mocked($fetch)).toHaveBeenCalledWith("/api/recipes");
    expect(recipes.value).toEqual(mockRecipes);
  });
});