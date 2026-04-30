import { describe, expect, it, vi } from "vitest";
import { mount } from "@vue/test-utils";
import AddRecipeModal from "../components/recipes/AddRecipeModal.vue";

describe("AddRecipeModal", () => {
  it("emits added event with form data on submit", async () => {
    vi.stubGlobal("useToast", () => ({
      pushSuccess: vi.fn(),
      pushError: vi.fn(),
    }));

    vi.stubGlobal("$fetch", vi.fn().mockResolvedValue({ id: "test-id" }));

    const wrapper = mount(AddRecipeModal, {
      global: {
        stubs: {
          SharedModal: { template: '<div class="stub-modal"><slot /><slot name="footer" /></div>' },
          SharedFormField: { template: '<div class="stub-field"><slot /></div>', props: ['label', 'error', 'required'] },
          SharedButton: { template: '<button class="stub-button"><slot /></button>', props: ['variant', 'disabled'] },
          Teleport: true,
        },
        mocks: {
          useRouter: () => ({ push: vi.fn() }),
          $fetch: vi.fn().mockResolvedValue({ id: "test-id" }),
        }
      }
    });

    await wrapper.find('input[placeholder*="Banana"]').setValue("Test Recipe");
    // Find the submit button directly by its text content
    const submitButton = wrapper.find("button:contains('Add Recipe')");
    await submitButton.trigger("click");

    expect(wrapper.emitted("added")).toBeTruthy();
  });
});