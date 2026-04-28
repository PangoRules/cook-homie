// src/CookHomie.Web/tests/useToast.spec.ts
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import { useToast } from "../composables/useToast";

describe("useToast", () => {
  beforeEach(() => {
    vi.stubGlobal("useState", (_key: string, init: () => unknown) => ref(init()));
  });

  it("pushes a success toast", () => {
    const { toasts, pushSuccess } = useToast();
    pushSuccess("Item added");
    expect(toasts.value).toContainEqual(
      expect.objectContaining({ type: "success", message: "Item added" })
    );
  });

  it("pushes an error toast with parsed message", () => {
    const { toasts, pushError } = useToast();
    pushError(new Error("Validation failed: name is required"));
    expect(toasts.value).toContainEqual(
      expect.objectContaining({
        type: "error",
        message: expect.stringContaining("name is required"),
      })
    );
  });

  it("clears all toasts", () => {
    const { toasts, pushSuccess, clear } = useToast();
    pushSuccess("One");
    clear();
    expect(toasts.value).toHaveLength(0);
  });

  it("removes a toast by id", () => {
    const { toasts, pushSuccess, remove } = useToast();
    pushSuccess("One");
    const id = toasts.value[0].id;
    remove(id);
    expect(toasts.value).toHaveLength(0);
  });
});
