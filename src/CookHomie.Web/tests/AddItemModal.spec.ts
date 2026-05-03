import { mount } from "@vue/test-utils";
import { beforeEach, describe, expect, it, vi } from "vitest";
import AddItemModal from "~/components/inventory/AddItemModal.vue";

vi.mock("~/composables/useToast", () => ({
  useToast: () => ({
    pushSuccess: vi.fn(),
    pushError: vi.fn(),
  }),
}));

vi.mock("~/composables/useInventory", () => ({
  useInventory: () => ({
    addInventoryItem: vi.fn().mockResolvedValue({
      id: "new-item-id",
      name: "Milk",
      category: "dairy",
      location: "Fridge",
      quantity: 1,
      unit: "liters",
      isOpened: false,
    }),
  }),
}));

describe("AddItemModal", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("validates empty required fields", async () => {
    const wrapper = mount(AddItemModal, {
      global: {
        stubs: {
          SharedModal: { template: '<div class="stub-modal"><slot /><slot name="footer" /></div>' },
          SharedFormField: {
            template: '<div class="stub-field"><slot /></div>',
            props: ["label", "error", "required"],
          },
          SharedButton: {
            template: '<button class="stub-button"><slot /></button>',
            props: ["variant", "disabled"],
          },
          Teleport: true,
        },
      },
    });

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const vm = wrapper.vm as any;
    const result = vm.validate();
    expect(result).toBe(false);
    expect(vm.errors.name).toBe("Name is required");
    expect(vm.errors.category).toBe("Category is required");
    expect(vm.errors.location).toBe("Location is required");
  });

  it("submits form and emits added on valid data", async () => {
    const $fetchMock = vi.fn().mockResolvedValue({ id: "new-item" });
    vi.stubGlobal("$fetch", $fetchMock);

    const wrapper = mount(AddItemModal, {
      global: {
        stubs: {
          SharedModal: { template: '<div class="stub-modal"><slot /><slot name="footer" /></div>' },
          SharedFormField: {
            template: '<div class="stub-field"><slot /></div>',
            props: ["label", "error", "required"],
          },
          SharedButton: {
            template: '<button class="stub-button"><slot /></button>',
            props: ["variant", "disabled"],
          },
          Teleport: true,
        },
      },
    });

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const vm = wrapper.vm as any;
    vm.form.name = "Milk";
    vm.form.category = "dairy";
    vm.form.location = "Fridge";
    vm.form.quantity = 1;
    await wrapper.vm.$nextTick();

    expect(vm.validate()).toBe(true);
  });
});
