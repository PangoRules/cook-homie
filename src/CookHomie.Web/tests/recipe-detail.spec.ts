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

  it("awaits initial recipe fetch when starting", async () => {
    let resolveRecipe!: (recipe: unknown) => void;
    vi.stubGlobal("$fetch", vi.fn(() => new Promise((resolve) => {
      resolveRecipe = resolve;
    })));

    const { useRecipeDetail } = await import("../composables/useRecipeDetail");
    const detail = useRecipeDetail("r2");
    const started = detail.start();

    expect(detail.recipe.value).toBeNull();
    await Promise.resolve();
    resolveRecipe({
      id: "r2",
      name: "Toast",
      instructions: "Toast.",
      prepMinutes: 1,
      cookMinutes: 2,
      tags: [],
      ingredients: [],
    });
    await started;

    expect(detail.recipe.value?.name).toBe("Toast");
  });
});
