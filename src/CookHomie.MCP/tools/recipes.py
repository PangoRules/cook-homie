from api_client import ApiClient

client = ApiClient()

async def get_recipes(query: str | None = None) -> dict:
    return {"recipes": []}