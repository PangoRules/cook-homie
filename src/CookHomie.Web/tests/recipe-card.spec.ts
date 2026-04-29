import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import RecipeCard from "../components/recipes/RecipeCard.vue";

describe("RecipeCard", () => {
  const recipe = {
    id: "r1",
    name: "Pancakes",
    instructions: "Mix and cook.",
    prepMinutes: 5,
    cookMinutes: 10,
    tags: ["breakfast"],
    ingredients: []
  };

  it("renders recipe name", () => {
    const wrapper = mount(RecipeCard, { props: { recipe } });
    expect(wrapper.find(".recipe-card__name").text()).toBe("Pancakes");
  });

  it("renders tags", () => {
    const wrapper = mount(RecipeCard, { props: { recipe } });
    expect(wrapper.find(".tag").text()).toBe("breakfast");
  });

  it("emits click with recipe id", async () => {
    const wrapper = mount(RecipeCard, { props: { recipe } });
    await wrapper.trigger("click");
    expect(wrapper.emitted("click")?.[0]).toEqual([recipe.id]);
  });
});