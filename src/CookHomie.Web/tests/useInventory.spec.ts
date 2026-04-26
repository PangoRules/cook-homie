import { describe, it, expect } from 'vitest'
import { useInventory } from '../composables/useInventory'

describe('useInventory', () => {
  it('should load inventory items', async () => {
    const { loadInventory } = useInventory()
    const inventory = await loadInventory()
    expect(inventory).toBeDefined()
  })

  it('should add inventory items', async () => {
    const { addInventoryItem } = useInventory()
    const newItem = await addInventoryItem({ name: 'Test Item', quantity: 1 })
    expect(newItem).toBeDefined()
  })
})