import { describe, expect, it, vi } from "vitest";
import { ref } from "vue";

describe("Dashboard", () => {
  it("uses useDashboard composable", async () => {
    vi.stubGlobal(
      "$fetch",
      vi.fn().mockResolvedValue({
        expiringCount: 3,
        recipeMatchCount: 7,
        shoppingCount: 4,
      })
    );
    vi.stubGlobal("useState", (_k: string, init: () => unknown) => ref(init()));
    vi.stubGlobal("usePollingFetch", () => ({
      data: ref({ expiringCount: 3, recipeMatchCount: 7, shoppingCount: 4 }),
      loading: ref(false),
      error: ref(null),
      isStale: ref(false),
      start: vi.fn(),
      stop: vi.fn(),
      refresh: vi.fn(),
    }));

    const { useDashboard } = await import("../composables/useDashboard");
    const { data } = useDashboard();
    expect(data.value?.expiringCount).toBe(3);
  });
});
