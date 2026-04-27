> **Status:** ✅ Completed and merged into main (2026-04-27).

# Web Add Item Flow Implementation Plan

**Goal:** Complete the UI flow for adding inventory items by implementing the missing modal, integrating with existing API, and validating through tests.

**Description:** Tighten the vertical slice for `add inventory item` by implementing the missing UI components (AddItemModal), updating composable logic, and verifying end-to-end flow with Cypress tests.

### Steps

- [x] **Step 1: Write failing modal submission test**
  ```ts
  describe("AddItemModal", () => {
    it("submits form and emits added event", async () => {
      const wrapper = mount(AddItemModal);
      await wrapper.find('input[name="name"]').setValue("Milk");
      await wrapper.find('button[type="submit"]').trigger("click");
      expect(wrapper.emitted("added")).toBeTruthy();
    });
  });
  ```

- [x] **Step 2: Run test to verify failure**
  ```bash
  cd src/CookHomie.Web && npm test -- AddItemModal.spec.ts
  ```
  Expected: Test fails (missing component/implementation).

- [x] **Step 3: Implement modal, composable, and proxy endpoint**
  ```vue
  <!-- components/inventory/AddItemModal.vue -->
  <template>
    <div>
      <form @submit.prevent="handleSubmit">
        <input v-model="form.name" name="name" />
        <button type="submit">Add</button>
      </form>
    </div>
  </template>
  ```
  ```ts
  // composables/useInventory.ts
  const addInventoryItem = async (payload: AddInventoryItemPayload) => {
    const created = await $fetch<InventoryItem>("/api/inventory", {
      method: "POST",
      body: payload,
    });
    items.value.unshift(created);
    return created;
  };
  ```
  ```ts
  // server/api/inventory/index.post.ts
  export default defineEventHandler(async (event) => {
    const body = await readBody(event);
    return await $fetch(`${process.env.API_BASE_URL}/api/inventory`, {
      method: "POST",
      body
    });
  });
  ```

- [x] **Step 4: Re-run tests**
  ```bash
  cd src/CookHomie.Web && npm test
  ```
  Expected: All tests pass (modal, composables, proxy).

- [x] **Step 5: Commit**
  ```bash
  git add src/CookHomie.Web
  git commit -m "feat: implement inventory add item modal and proxy"
  ```