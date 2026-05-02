import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import RecipesDetailIngredientStockBadge from "../components/recipes/detail/IngredientStockBadge.vue";

describe("RecipesDetailIngredientStockBadge", () => {
  it("shows 'In stock' when isInStock is true", () => {
    const wrapper = mount(RecipesDetailIngredientStockBadge, { props: { isInStock: true } });
    expect(wrapper.text()).toContain("In stock");
    expect(wrapper.find(".bg-success-subtle").exists()).toBe(true);
  });

  it("shows 'Missing' when isInStock is false", () => {
    const wrapper = mount(RecipesDetailIngredientStockBadge, { props: { isInStock: false } });
    expect(wrapper.text()).toContain("Missing");
    expect(wrapper.find(".bg-error-subtle").exists()).toBe(true);
  });
});