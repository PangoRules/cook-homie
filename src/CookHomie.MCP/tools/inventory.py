from api_client import ApiClient

client = ApiClient()  # Uses API_BASE_URL env var

async def get_inventory(location: str | None = None) -> dict:
    items = await client.get_inventory(location=location)
    return {"items": items, "count": len(items)}