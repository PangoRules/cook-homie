import { mount } from "@vue/test-utils";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { ref } from "vue";
import IndexPage from "../pages/index.vue";

describe("index page", () => {
  beforeEach(() => {
    vi.stubGlobal("useDashboard", () => ({
      data: ref({ expiringCount: 0, recipeMatchCount: 0, shoppingCount: 0 }),
      loading: ref(false),
      error: ref(null),
      isStale: ref(false),
      start: vi.fn(),
      stop: vi.fn(),
      refresh: vi.fn(),
    }));
  });

  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("renders heading and dashboard content", () => {
    const wrapper = mount(IndexPage, {
      global: {
        stubs: {
          DashboardStatCard: { 
            template: '<div class="stat-card"><span>{{ label }}</span></div>', 
            props: ['label', 'value', 'sub', 'variant'] 
          },
          DashboardPanel: { 
            template: '<div class="panel"><h2>{{ title }}</h2><slot /></div>', 
            props: ['title', 'loading', 'error', 'isStale', 'hasData', 'showRefresh'] 
          },
        }
      }
    });

    expect(wrapper.find("h1").text()).toBe("Kitchen Overview");
    expect(wrapper.text()).toContain("Expiring Soon");
    expect(wrapper.text()).toContain("Recipe Matches");
    expect(wrapper.text()).toContain("Shopping List");
  });

  it("renders loading state when data is not available", () => {
    vi.stubGlobal("useDashboard", () => ({
      data: ref(null),
      loading: ref(true),
      error: ref(null),
      isStale: ref(false),
      start: vi.fn(),
      stop: vi.fn(),
      refresh: vi.fn(),
    }));

    const wrapper = mount(IndexPage, {
      global: {
        stubs: {
          DashboardStatCard: { 
            template: '<div class="stat-card"><span>{{ label }}</span></div>', 
            props: ['label', 'value', 'sub', 'variant'] 
          },
          DashboardPanel: { 
            template: '<div class="panel"><h2>{{ title }}</h2><slot /></div>', 
            props: ['title', 'loading', 'error', 'isStale', 'hasData', 'showRefresh'] 
          },
        }
      }
    });

    expect(wrapper.text()).toContain("Kitchen Overview");
  });

  it("renders error state when fetch fails", () => {
    vi.stubGlobal("useDashboard", () => ({
      data: ref(null),
      loading: ref(false),
      error: ref(new Error("upstream failure")),
      isStale: ref(false),
      start: vi.fn(),
      stop: vi.fn(),
      refresh: vi.fn(),
    }));

    const wrapper = mount(IndexPage, {
      global: {
        stubs: {
          DashboardStatCard: { 
            template: '<div class="stat-card"><span>{{ label }}</span></div>', 
            props: ['label', 'value', 'sub', 'variant'] 
          },
          DashboardPanel: { 
            template: '<div class="panel"><h2>{{ title }}</h2><slot /></div>', 
            props: ['title', 'loading', 'error', 'isStale', 'hasData', 'showRefresh'] 
          },
        }
      }
    });

    expect(wrapper.text()).toContain("Kitchen Overview");
  });
});
