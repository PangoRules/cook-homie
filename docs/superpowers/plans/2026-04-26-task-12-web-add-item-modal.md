# Task 12 Implementation Plan: Web Add Item Modal

## Purpose
Implement the inventory item addition modal for the web interface, completing the vertical slice for item management.

## Implementation Steps

1. **Create modal component**
   - Location: `src/CookHomie.Web/components/Inventory/AddItemModal.vue`
   - Include form fields for: `name`, `quantity`, `unit` (dropdown), `expiryDate` (date picker), `tags` (tags input)
   - Use Nuxt's `v-model` for form state management
   - Implement basic validation (name required, quantity > 0)

2. **Implement form handling composables**
   - Location: `src/CookHomie.Web/composables/inventory/useAddItemForm.ts`
   - Establish API call to `@/server/api/inventory` endpoint
   - Handle success/error states for API responses
   - Include loading indicators during submission

3. **Integrate modal into inventory page**
   - Add 'New Item' button in `src/CookHomie.Web/pages/inventory.vue`
   - Bind button click to modal open state
   - Add reset handler for new item entry after successful submission

4. **Verify end-to-end flow**
   - Test modal opening/closing
   - Test form submission with valid data
   - Test error handling for invalid inputs
   - Confirm API mutation triggers inventory update

## Parallel Work
- Component implementation (Step 1) and API handler implementation (Step 2) can be worked on concurrently
- Use subagent-driven-development for parallel work on these tasks

## Testing Requirements
- Verify modal appears on button click
- Confirm form submission triggers API call with correct payload
- Check error message display for invalid entries
- Validate success flow shows item list update