import { mount } from "@vue/test-utils";
import { describe, expect, it } from "vitest";
import IngredientTable from "../components/shared/IngredientTable.vue";
import type { AddRecipeIngredientPayload } from "../types";

interface MountIngredientTableOptions {
  readonly pageSize?: number;
  readonly startEditingFirstRow?: boolean;
}

const makeIngredient = (ingredientName: string): AddRecipeIngredientPayload => ({
  ingredientName,
  quantity: 1,
  unit: "g",
  isOptional: false,
});

const mountIngredientTable = (ingredients: AddRecipeIngredientPayload[], options: MountIngredientTableOptions = {}) =>
  mount(IngredientTable, {
    props: {
      ingredients,
      mode: "editable",
      pageSize: options.pageSize ?? 5,
      startEditingFirstRow: options.startEditingFirstRow,
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
    const wrapper = mountIngredientTable(ingredients, { pageSize: 2 });

    await wrapper.findAll("button").find((button) => button.text() === "Next")?.trigger("click");
    await wrapper.findAll("button").find((button) => button.text() === "Remove")?.trigger("click");

    expect(wrapper.emitted("remove")?.[0]).toEqual([2]);
  });

  it("opens an expanded editor and requires an ingredient name before finishing", async () => {
    const ingredients = [makeIngredient("")];
    const wrapper = mountIngredientTable(ingredients, { startEditingFirstRow: true });

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

  it("keeps editable tables compact by default", () => {
    const ingredients = [makeIngredient("Flour")];
    const wrapper = mountIngredientTable(ingredients);

    expect(wrapper.find('input[placeholder="e.g. Flour"]').exists()).toBe(false);
    expect(wrapper.text()).toContain("Edit");
    expect(wrapper.text()).toContain("Remove");
  });

  it("opens the first row when startEditingFirstRow is enabled", () => {
    const ingredients = [makeIngredient("Flour")];
    const wrapper = mountIngredientTable(ingredients, { startEditingFirstRow: true });

    expect(wrapper.find<HTMLInputElement>('input[placeholder="e.g. Flour"]').element.value).toBe("Flour");
  });

  it("cancels editing without saving or removing", async () => {
    const ingredients = [makeIngredient("Flour")];
    const wrapper = mountIngredientTable(ingredients, { startEditingFirstRow: true });

    await wrapper.find('input[placeholder="e.g. Flour"]').setValue("Sugar");
    await wrapper.findAll("button").find((button) => button.text() === "Cancel")?.trigger("click");

    expect(ingredients[0]?.ingredientName).toBe("Flour");
    expect(wrapper.emitted("remove")).toBeUndefined();
    expect(wrapper.find('input[placeholder="e.g. Flour"]').exists()).toBe(false);
    expect(wrapper.text()).toContain("Flour");
  });

  it("hides pagination controls until more than one page is needed", () => {
    const ingredients = [makeIngredient("Flour"), makeIngredient("Milk")];
    const wrapper = mountIngredientTable(ingredients, { pageSize: 5 });

    expect(wrapper.text()).not.toContain("Prev");
    expect(wrapper.text()).not.toContain("Next");
  });

  it("shows pagination controls when more than one page is needed", () => {
    const ingredients = ["one", "two", "three"].map(makeIngredient);
    const wrapper = mountIngredientTable(ingredients, { pageSize: 2 });

    expect(wrapper.text()).toContain("Prev");
    expect(wrapper.text()).toContain("Next");
  });
});
