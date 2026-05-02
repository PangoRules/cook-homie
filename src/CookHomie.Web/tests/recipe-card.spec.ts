import { describe, expect, it, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { defineComponent, h } from "vue";
import RecipeCard from "../components/recipes/RecipeCard.vue";

const createNuxtLinkStub = (onNavigate?: (to: string) => void) =>
  defineComponent({
    props: {
      to: {
        type: String,
        required: true,
      },
    },
    setup(props, { slots }) {
      return () =>
        h(
          "a",
          {
            href: props.to,
            onClick: (event: Event) => {
              event.preventDefault();
              onNavigate?.(props.to);
            },
          },
          slots.default?.()
        );
    },
  });

const mountRecipeCard = (onNavigate?: (to: string) => void) =>
  mount(RecipeCard, {
    props: { recipe },
    global: { stubs: { NuxtLink: createNuxtLinkStub(onNavigate) } },
  });

const recipe = {
  id: "r1",
  name: "Pancakes",
  instructions: "Mix and cook.",
  prepMinutes: 5,
  cookMinutes: 10,
  tags: ["breakfast"],
  ingredients: [],
};

describe("RecipeCard", () => {
  it("renders recipe name", () => {
    const wrapper = mountRecipeCard();
    expect(wrapper.find("h3").text()).toBe("Pancakes");
  });

  it("renders tags", () => {
    const wrapper = mountRecipeCard();
    expect(wrapper.findAll("span").length).toBe(1);
    expect(wrapper.findAll("span")[0].text()).toBe("breakfast");
  });

  it("navigates to recipe detail on click", async () => {
    const push = vi.fn();
    const wrapper = mountRecipeCard(push);

    await wrapper.find("a").trigger("click");

    expect(push).toHaveBeenCalledOnce();
    expect(push).toHaveBeenCalledWith("recipes/r1");
  });

  it("NuxtLink has correct href", () => {
    const wrapper = mountRecipeCard();
    expect(wrapper.find("a").attributes("href")).toBe("recipes/r1");
  });
});
