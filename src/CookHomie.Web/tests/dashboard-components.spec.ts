import { describe, expect, it, vi } from "vitest";
import { mount } from "@vue/test-utils";
import DashboardStatCard from "../components/dashboard/DashboardStatCard.vue";
import DashboardPanel from "../components/dashboard/DashboardPanel.vue";

vi.mock("../composables/useLocale", () => ({
  useLocale: () => ({
    locale: { value: "en-US" },
    setLocale: () => {},
    formatNumber: (v: number | string) =>
      typeof v === "number" ? v.toLocaleString("en-US") : v,
  }),
}));

describe("DashboardStatCard", () => {
  const mountOptions = {
    global: {
      stubs: {
        SharedSkeletonBlock: { template: '<div class="stub-skeleton" />' },
      },
    },
  };

  it("renders label and value", () => {
    const wrapper = mount(DashboardStatCard, { ...mountOptions, props: { label: "Expiring", value: 3, loading: false } });
    expect(wrapper.find(".font-semibold.uppercase").text()).toBe("Expiring");
    expect(wrapper.find(".text-\\[26px\\]").text()).toBe("3");
  });

  it("formats number values with locale", () => {
    const wrapper = mount(DashboardStatCard, { ...mountOptions, props: { label: "Items", value: 1234, loading: false } });
    expect(wrapper.find(".text-\\[26px\\]").text()).toBe("1,234");
  });

  it("applies warning variant class", () => {
    const wrapper = mount(DashboardStatCard, { ...mountOptions, props: { label: "Warn", value: 5, variant: "warning", loading: false } });
    expect(wrapper.classes()).toContain("border-l-4");
    expect(wrapper.classes()).toContain("border-l-warning");
  });
});

describe("DashboardPanel", () => {
  const mountOptions = {
    global: {
      stubs: {
        SharedSkeletonBlock: { template: '<div class="stub-skeleton" />' },
        SharedErrorBanner: { template: '<div class="stub-error-banner" />' },
        SharedStaleIndicator: { template: '<div class="stub-stale-indicator" />' },
      },
    },
  };

  it("renders title", () => {
    const wrapper = mount(DashboardPanel, { ...mountOptions, props: { title: "My Panel" } });
    expect(wrapper.find("h2").text()).toBe("My Panel");
  });

  it("shows refresh button when showRefresh is true", () => {
    const wrapper = mount(DashboardPanel, { ...mountOptions, props: { title: "P", showRefresh: true, loading: false, hasData: true } });
    expect(wrapper.find("button").exists()).toBe(true);
  });

  it("emits refresh when refresh button clicked", async () => {
    const wrapper = mount(DashboardPanel, { ...mountOptions, props: { title: "P", showRefresh: true, loading: false, hasData: true } });
    await wrapper.find("button").trigger("click");
    expect(wrapper.emitted("refresh")).toBeTruthy();
  });
});