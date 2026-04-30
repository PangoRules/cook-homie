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
});