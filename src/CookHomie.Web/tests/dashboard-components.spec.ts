import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import DashboardStatCard from "../components/dashboard/DashboardStatCard.vue";
import DashboardPanel from "../components/dashboard/DashboardPanel.vue";

describe("DashboardStatCard", () => {
  it("renders label and value", () => {
    const wrapper = mount(DashboardStatCard, { props: { label: "Expiring", value: 3 } });
    expect(wrapper.find(".font-semibold.uppercase").text()).toBe("Expiring");
    expect(wrapper.find(".text-\\[26px\\]").text()).toBe("3");
  });

  it("formats number values with locale", () => {
    const wrapper = mount(DashboardStatCard, { props: { label: "Items", value: 1234 } });
    expect(wrapper.find(".text-\\[26px\\]").text()).toBe("1,234");
  });

  it("applies warning variant class", () => {
    const wrapper = mount(DashboardStatCard, { props: { label: "Warn", value: 5, variant: "warning" } });
    expect(wrapper.classes()).toContain("border-l-4");
    expect(wrapper.classes()).toContain("border-l-warning");
  });
});

describe("DashboardPanel", () => {
  it("renders title", () => {
    const wrapper = mount(DashboardPanel, { props: { title: "My Panel" } });
    expect(wrapper.find("h2").text()).toBe("My Panel");
  });

  it("shows refresh button when showRefresh is true", () => {
    const wrapper = mount(DashboardPanel, { props: { title: "P", showRefresh: true, loading: false, hasData: true } });
    expect(wrapper.find("button").exists()).toBe(true);
  });

  it("emits refresh when refresh button clicked", async () => {
    const wrapper = mount(DashboardPanel, { props: { title: "P", showRefresh: true, loading: false, hasData: true } });
    await wrapper.find("button").trigger("click");
    expect(wrapper.emitted("refresh")).toBeTruthy();
  });
});