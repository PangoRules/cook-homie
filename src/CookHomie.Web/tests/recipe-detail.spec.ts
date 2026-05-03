import { beforeEach, describe, expect, it, vi } from "vitest";
import { computed, ref } from "vue";

const loadInventoryMock = vi.hoisted(() => vi.fn());

vi.mock("../composables/useInventory", async () => {
  const { ref } = await import("vue");

  return {
    useInventory: () => ({
      items: ref([{ id: "inv-1", name: "Milk" }]),
      loadInventory: loadInventoryMock,
    }),
  };
});

describe("useRecipeDetail", () => {
  beforeEach(() => {
    loadInventoryMock.mockReset();
    loadInventoryMock.mockResolvedValue(undefined);
    vi.stubGlobal("computed", computed);
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));
  });

  it("loads inventory before recipe enrichment", async () => {
    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue({
      id: "r1",
      name: "Pancakes",
      instructions: "Mix.",
      prepMinutes: 5,
      cookMinutes: 10,
      tags: [],
      ingredients: [
        { id: "i1", recipeId: "r1", ingredientName: "milk", quantity: 250, unit: "ml", isOptional: false },
      ],
    }));

    const { useRecipeDetail } = await import("../composables/useRecipeDetail");
    const detail = useRecipeDetail("r1");
    await detail.start();

    expect(loadInventoryMock).toHaveBeenCalled();
    expect(detail.enrichedIngredients.value[0].isInStock).toBe(true);
  });
});
