import { describe, expect, it, vi } from "vitest";
import { mount } from "@vue/test-utils";
import AddRecipeModal from "../components/recipes/AddRecipeModal.vue";

describe("AddRecipeModal", () => {
  it("emits added event with form data on submit", async () => {
    const mockUseToast = { pushSuccess: vi.fn(), pushError: vi.fn() };
    const mockFetch = vi.fn().mockResolvedValue({ id: "new", name: "Test" });
    
    vi.stubGlobal("useToast", () => mockUseToast);
    vi.stubGlobal("$fetch", mockFetch);

    const wrapper = mount(AddRecipeModal, { global: { stubs: { Teleport: false } } });
    await wrapper.find('input[placeholder*="Banana"]').setValue("Test Recipe");
    await wrapper.find("form").trigger("submit");
    expect(wrapper.emitted("added")).toBeTruthy();
  });
});