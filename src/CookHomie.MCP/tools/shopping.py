from api_client import ApiClient

client = ApiClient()

async def get_shopping_list() -> dict:
    return {"items": []}