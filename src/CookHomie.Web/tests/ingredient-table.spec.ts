import { mount } from "@vue/test-utils";
import { describe, expect, it } from "vitest";
import IngredientTable from "../components/shared/IngredientTable.vue";
import type { AddRecipeIngredientPayload } from "../types";

const makeIngredient = (ingredientName: string): AddRecipeIngredientPayload => ({
  ingredientName,
  quantity: 1,
  unit: "g",
  isOptional: false,
});

const mountIngredientTable = (ingredients: AddRecipeIngredientPayload[], pageSize = 5) =>
  mount(IngredientTable, {
    props: {
      ingredients,
      mode: "editable",
      pageSize,
    },
    global: {
      stubs: {
        SharedButton: {
          template: '<button type="button"><slot /></button>',
          props: ["variant", "disabled", "size"],
        },
        SharedFormField: {
          template: '<label><span>{{ label }}</span><slot /><small>{{ error }}</small></label>',
          props: ["label", "error", "required"],
        },
      },
    },
  });

describe("IngredientTable", () => {
  it("emits original ingredient index when removing an item on a later page", async () => {
    const ingredients = ["one", "two", "three"].map(makeIngredient);
    const wrapper = mountIngredientTable(ingredients, 2);

    await wrapper.findAll("button").find((button) => button.text() === "Next")?.trigger("click");
    await wrapper.findAll("button").find((button) => button.text() === "Remove")?.trigger("click");

    expect(wrapper.emitted("remove")?.[0]).toEqual([2]);
  });

  it("opens an expanded editor and requires an ingredient name before finishing", async () => {
    const ingredients = [makeIngredient("")];
    const wrapper = mountIngredientTable(ingredients);

    await wrapper.findAll("button").find((button) => button.text() === "Done")?.trigger("click");

    expect(wrapper.text()).toContain("Ingredient name is required");

    await wrapper.find('input[placeholder="e.g. Flour"]').setValue("Flour");
    await wrapper.findAll("button").find((button) => button.text() === "Done")?.trigger("click");

    expect(wrapper.text()).not.toContain("Ingredient name is required");
    expect(wrapper.text()).toContain("Flour");
  });

  it("exposes editIngredient to open the editor for a specific row", async () => {
    const ingredients = [makeIngredient("Flour"), makeIngredient("Milk")];
    const wrapper = mountIngredientTable(ingredients);

    await wrapper.vm.editIngredient(1);

    expect(wrapper.find<HTMLInputElement>('input[placeholder="e.g. Flour"]').element.value).toBe("Milk");
  });
});
