import { mount } from "@vue/test-utils";
import { beforeEach, describe, expect, it, vi } from "vitest";
import type { AddRecipeIngredientPayload } from "~/types";
import { makeRecipeIngredient } from "./helpers/builders";
import { sharedStubs } from "./helpers/stubs";
import AddRecipeModal from "../components/recipes/AddRecipeModal.vue";

const mountAddRecipeModal = () =>
  mount(AddRecipeModal, {
    global: {
      stubs: sharedStubs,
    },
  });

const fillValidRecipeForm = (vm: AddRecipeModalVm) => {
  vm.form.name = "Test Recipe";
  vm.form.instructions = "Test instructions";
  vm.form.prepMinutes = 10;
  vm.form.cookMinutes = 20;
  vm.form.ingredients = [makeRecipeIngredient()];
};

type AddRecipeModalVm = InstanceType<typeof AddRecipeModal> & {
  validate: () => boolean;
  handleSubmit: () => Promise<void>;
  form: {
    name: string;
    prepMinutes: number;
    cookMinutes: number;
    instructions: string;
    tags: string[];
    ingredients: AddRecipeIngredientPayload[];
  };
  errors: {
    name: string;
    ingredients: string;
  };
};

vi.mock("@/composables/useToast", () => ({
  useToast: () => ({
    pushSuccess: vi.fn(),
    pushError: vi.fn(),
  }),
}));

describe("AddRecipeModal", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("validates form and shows error for empty name", async () => {
    const wrapper = mountAddRecipeModal();

    const vm = wrapper.vm as AddRecipeModalVm;
    const result = vm.validate();

    expect(result).toBe(false);
    expect(vm.errors.name).toBe("Recipe name is required");
  });

  it("requires at least one ingredient", async () => {
    const wrapper = mountAddRecipeModal();

    const vm = wrapper.vm as AddRecipeModalVm;
    fillValidRecipeForm(vm);
    vm.form.ingredients = [makeRecipeIngredient({ ingredientName: "", unit: "" })];

    expect(vm.validate()).toBe(false);
    expect(vm.errors.ingredients).toBe("At least one ingredient is required");
  });

  it("emits added event when form is valid", async () => {
    const $fetchMock = vi.fn().mockResolvedValue({ id: "test-id" });
    vi.stubGlobal("$fetch", $fetchMock);

    const wrapper = mountAddRecipeModal();

    const vm = wrapper.vm as AddRecipeModalVm;
    fillValidRecipeForm(vm);
    await wrapper.vm.$nextTick();

    expect(vm.validate()).toBe(true);
    await vm.handleSubmit();
    await wrapper.vm.$nextTick();

    expect($fetchMock).toHaveBeenCalled();
    expect(wrapper.emitted("added")).toBeTruthy();
  });

  it("submits ingredients when provided", async () => {
    const $fetchMock = vi.fn().mockResolvedValue({ id: "test-id" });
    vi.stubGlobal("$fetch", $fetchMock);

    const wrapper = mountAddRecipeModal();

    const vm = wrapper.vm as AddRecipeModalVm;
    fillValidRecipeForm(vm);
    vm.form.name = "Test Recipe with Ingredients";
    vm.form.tags = ["dinner", "quick"];
    vm.form.ingredients = [
      makeRecipeIngredient({ ingredientName: " flour ", unit: " g " }),
      makeRecipeIngredient({ ingredientName: "eggs", quantity: 2, unit: "pcs", isOptional: true }),
    ];

    await wrapper.vm.$nextTick();

    expect(vm.validate()).toBe(true);
    await vm.handleSubmit();
    await wrapper.vm.$nextTick();

    // Check that ingredients are included in the submitted data
    expect($fetchMock).toHaveBeenCalledWith(
      "/api/recipes",
      expect.objectContaining({
        body: expect.objectContaining({
          ingredients: [
            { ingredientName: "flour", quantity: 200, unit: "g", isOptional: false },
            { ingredientName: "eggs", quantity: 2, unit: "pcs", isOptional: true },
          ],
        }),
      })
    );
  });
});
