export const sharedStubs = {
  SharedModal: {
    template: '<div class="stub-modal"><slot /><slot name="footer" /></div>',
  },
  SharedFormField: {
    template: '<div class="stub-field"><slot /></div>',
    props: ["label", "error", "required"],
  },
  SharedButton: {
    template: '<button class="stub-button"><slot /></button>',
    props: ["variant", "disabled", "size"],
  },
  SharedTagInput: {
    template: '<input class="stub-tag-input" />',
  },
  IngredientTable: {
    template: '<div class="stub-ingredient-table" />',
    props: ["ingredients", "mode", "pageSize"],
    emits: ["remove", "update:ingredients"],
  },
  Teleport: true,
};
