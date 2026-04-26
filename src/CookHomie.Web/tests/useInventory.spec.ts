import { beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { useInventory } from "../composables/useInventory";

describe("useInventory", () => {
  beforeEach(() => {
    vi.unstubAllGlobals();
  });

  it("loads inventory items from /api/inventory into local state", async () => {
    const mockItems = [
      {
        id: "1",
        name: "Milk",
        category: "dairy",
        location: "Fridge",
        quantity: 1,
        unit: "liter",
        isOpened: false
      }
    ];
    const fetchSpy = vi.fn().mockResolvedValue(mockItems);

    vi.stubGlobal("$fetch", fetchSpy);
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

    const { items, loadInventory } = useInventory();
    await loadInventory();

    expect(fetchSpy).toHaveBeenCalledWith("/api/inventory");
    expect(items.value).toEqual(mockItems);
  });

  it("posts a new item and prepends it to local state", async () => {
    const created = {
      id: "2",
      name: "Eggs",
      category: "dairy",
      location: "Fridge",
      quantity: 12,
      unit: "units",
      isOpened: false
    };
    const fetchSpy = vi.fn().mockResolvedValue(created);

    vi.stubGlobal("$fetch", fetchSpy);
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));

    const payload = {
      name: "Eggs",
      category: "dairy",
      location: "Fridge",
      quantity: 12,
      unit: "units",
      isOpened: false
    };

    const { items, addInventoryItem } = useInventory();
    const result = await addInventoryItem(payload);

    expect(fetchSpy).toHaveBeenCalledWith("/api/inventory", {
      method: "POST",
      body: payload
    });
    expect(result).toEqual(created);
    expect(items.value[0]).toEqual(created);
  });
});
