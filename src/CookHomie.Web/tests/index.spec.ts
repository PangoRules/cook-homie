import { mount } from "@vue/test-utils";
import { afterEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import IndexPage from "../pages/index.vue";

describe("index page", () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("renders heading and api message on success", () => {
    vi.stubGlobal("useFetch", () => ({
      data: ref({ message: "Hello from C#" }),
      error: ref(null)
    }));

    const wrapper = mount(IndexPage);

    expect(wrapper.find("h1").text()).toBe("CookHomie Spike Web");
    expect(wrapper.text()).toContain("Message from API: Hello from C#");
  });

  it("renders loading state when message is not available", () => {
    vi.stubGlobal("useFetch", () => ({
      data: ref(null),
      error: ref(null)
    }));

    const wrapper = mount(IndexPage);

    expect(wrapper.text()).toContain("Loading spike message...");
  });

  it("renders error state when fetch fails", () => {
    vi.stubGlobal("useFetch", () => ({
      data: ref(null),
      error: ref(new Error("upstream failure"))
    }));

    const wrapper = mount(IndexPage);

    expect(wrapper.text()).toContain("Unable to load spike message.");
  });
});
