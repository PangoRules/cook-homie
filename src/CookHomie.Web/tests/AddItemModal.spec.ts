import { describe, it, expect } from "vitest";
import { mount } from "@vue/test-utils";
import AddItemModal from "../components/inventory/AddItemModal.vue";

describe("AddItemModal", () => {
  it("submits form and emits added event", async () => {
    const wrapper = mount(AddItemModal);
    await wrapper.find('input[name="name"]').setValue("Milk");
    await wrapper.find('button[type="submit"]').trigger("click");
    expect(wrapper.emitted("added")).toBeTruthy();
    expect(wrapper.emitted("added")?.[0][0]).toEqual({ name: "Milk" });
  });
});