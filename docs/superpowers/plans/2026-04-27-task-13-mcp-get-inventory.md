# Task 13: Implement MCP `get_inventory` Tool

> **Status:** ✓ Complete (2026-04-27).
> **Gap found:** `tools/inventory.py` returns `{"items": items}` but plan specifies `{"items": items, "count": len(items)}`. No other gaps found.

---

## Files to Modify

- `src/CookHomie.MCP/tools/inventory.py`
- `src/CookHomie.MCP/tests/test_inventory_tool.py`

---

## Step 1: Add failing `count` assertion to existing test

**File:** `src/CookHomie.MCP/tests/test_inventory_tool.py`

Add `count` assertion to `test_get_inventory_returns_items_dict`:

```python
@pytest.mark.asyncio
async def test_get_inventory_returns_items_dict():
    with patch("tools.inventory.client") as mock_client:
        mock_client.get_inventory = AsyncMock(return_value=[
            {"id": "1", "name": "Milk", "quantity": 1, "unit": "liter"}
        ])
        result = await get_inventory()
        assert "items" in result
        assert len(result["items"]) == 1
        assert result["items"][0]["name"] == "Milk"
        assert result["count"] == 1  # ← ADD THIS
```

Run: `cd src/CookHomie.MCP && pytest tests/test_inventory_tool.py::test_get_inventory_returns_items_dict -v`
Expected: FAIL — `count` key not in result

---

## Step 2: Update `get_inventory` to include `count`

**File:** `src/CookHomie.MCP/tools/inventory.py`

```python
async def get_inventory(location: str | None = None) -> dict:
    items = await client.get_inventory(location=location)
    return {"items": items, "count": len(items)}  # ← ADD count
```

Run: `cd src/CookHomie.MCP && pytest tests/test_inventory_tool.py -v`
Expected: PASS

---

## Step 3: Commit

```bash
cd /home/pango/Projects/cook-homie
git add src/CookHomie.MCP/tools/inventory.py src/CookHomie.MCP/tests/test_inventory_tool.py
git commit -m "feat: add count field to mcp get_inventory tool response"
```

---

## Verification

```bash
cd /home/pango/Projects/cook-homie/src/CookHomie.MCP && pytest tests/test_inventory_tool.py -v
```

Expected: 3 passed