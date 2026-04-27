import pytest
from unittest.mock import AsyncMock, patch
from tools.inventory import get_inventory


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
        assert result["count"] == 1


@pytest.mark.asyncio
async def test_get_inventory_passes_location_filter():
    with patch("tools.inventory.client") as mock_client:
        mock_client.get_inventory = AsyncMock(return_value=[])
        await get_inventory(location="Fridge")
        mock_client.get_inventory.assert_called_once_with(location="Fridge")


@pytest.mark.asyncio
async def test_get_inventory_handles_empty_result():
    with patch("tools.inventory.client") as mock_client:
        mock_client.get_inventory = AsyncMock(return_value=[])
        result = await get_inventory()
        assert result["items"] == []