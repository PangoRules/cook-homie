import { mount } from "@vue/test-utils";
import { describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import IndexPage from "../pages/index.vue";

describe("index page", () => {
  it("renders the spike heading", () => {
    vi.stubGlobal("useFetch", () => ({
      data: ref({ message: "Hello from C#" }),
      error: ref(null)
    }));

    const wrapper = mount(IndexPage);

    expect(wrapper.find("h1").text()).toBe("CookHomie Spike Web");
    expect(wrapper.text()).toContain("Message from API: Hello from C#");

    vi.unstubAllGlobals();
  });
});
