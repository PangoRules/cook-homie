import { mount } from "@vue/test-utils";
import { beforeEach, describe, expect, it, vi } from "vitest";
import AddRecipeModal from "../components/recipes/AddRecipeModal.vue";

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
    const wrapper = mount(AddRecipeModal, {
      global: {
        stubs: {
          SharedModal: { template: '<div class="stub-modal"><slot /><slot name="footer" /></div>' },
          SharedFormField: { template: '<div class="stub-field"><slot /></div>', props: ['label', 'error', 'required'] },
          SharedButton: { template: '<button class="stub-button"><slot /></button>', props: ['variant', 'disabled'] },
          SharedTagInput: { template: '<input class="stub-tag-input" />' },
          Teleport: true,
        }
      }
    });

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const vm = wrapper.vm as any;
    const result = vm.validate();
    
    expect(result).toBe(false);
    expect(vm.errors.name).toBe("Recipe name is required");
  });

  it("emits added event when form is valid", async () => {
    const $fetchMock = vi.fn().mockResolvedValue({ id: "test-id" });
    vi.stubGlobal("$fetch", $fetchMock);

    const wrapper = mount(AddRecipeModal, {
      global: {
        stubs: {
          SharedModal: { template: '<div class="stub-modal"><slot /><slot name="footer" /></div>' },
          SharedFormField: { template: '<div class="stub-field"><slot /></div>', props: ['label', 'error', 'required'] },
          SharedButton: { template: '<button class="stub-button"><slot /></button>', props: ['variant', 'disabled'] },
          SharedTagInput: { template: '<input class="stub-tag-input" />' },
          Teleport: true,
        },
      }
    });

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const vm = wrapper.vm as any;
    vm.form.name = "Test Recipe";
    vm.form.instructions = "Test instructions";
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

    const wrapper = mount(AddRecipeModal, {
      global: {
        stubs: {
          SharedModal: { template: '<div class="stub-modal"><slot /><slot name="footer" /></div>' },
          SharedFormField: { template: '<div class="stub-field"><slot /></div>', props: ['label', 'error', 'required'] },
          SharedButton: { template: '<button class="stub-button"><slot /></button>', props: ['variant', 'disabled'] },
          SharedTagInput: { template: '<input class="stub-tag-input" />' },
          Teleport: true,
        },
      }
    });

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const vm = wrapper.vm as any;
    vm.form.name = "Test Recipe with Ingredients";
    vm.form.instructions = "Test instructions";
    vm.form.prepMinutes = 10;
    vm.form.cookMinutes = 20;
    vm.form.tags = ["dinner", "quick"];
    vm.form.ingredients = [
      { ingredientName: "flour", quantity: 200, unit: "g" },
      { ingredientName: "eggs", quantity: 2, unit: "pcs" }
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
            { ingredientName: "flour", quantity: 200, unit: "g" },
            { ingredientName: "eggs", quantity: 2, unit: "pcs" }
          ]
        })
      })
    );
  });
});