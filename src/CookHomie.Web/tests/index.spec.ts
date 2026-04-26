import { mount } from "@vue/test-utils";
import { describe, expect, it } from "vitest";
import IndexPage from "../pages/index.vue";

describe("index page", () => {
  it("renders the spike heading", () => {
    const wrapper = mount(IndexPage);

    expect(wrapper.find("h1").text()).toBe("CookHomie Spike Web");
  });
});
