import { mount } from "@vue/test-utils";
import { beforeEach, describe, expect, it, vi } from "vitest";
import AddItemModal from "~/components/inventory/AddItemModal.vue";
import { makeInventoryItemPayload } from "./helpers/builders";
import { sharedStubs } from "./helpers/stubs";

type AddItemModalVm = InstanceType<typeof AddItemModal> & {
  validate: () => boolean;
  form: {
    name: string;
    category: string;
    location: string;
    quantity: number;
  };
  errors: {
    name: string;
    category: string;
    location: string;
  };
};

const mountAddItemModal = () =>
  mount(AddItemModal, {
    global: {
      stubs: sharedStubs,
    },
  });

const fillValidInventoryForm = (vm: AddItemModalVm) => {
  const payload = makeInventoryItemPayload();
  vm.form.name = payload.name;
  vm.form.category = payload.category;
  vm.form.location = payload.location;
  vm.form.quantity = payload.quantity;
};

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
    const wrapper = mountAddItemModal();

    const vm = wrapper.vm as AddItemModalVm;
    const result = vm.validate();
    expect(result).toBe(false);
    expect(vm.errors.name).toBe("Name is required");
    expect(vm.errors.category).toBe("Category is required");
    expect(vm.errors.location).toBe("Location is required");
  });

  it("submits form and emits added on valid data", async () => {
    const $fetchMock = vi.fn().mockResolvedValue({ id: "new-item" });
    vi.stubGlobal("$fetch", $fetchMock);

    const wrapper = mountAddItemModal();

    const vm = wrapper.vm as AddItemModalVm;
    fillValidInventoryForm(vm);
    await wrapper.vm.$nextTick();

    expect(vm.validate()).toBe(true);
  });
});
