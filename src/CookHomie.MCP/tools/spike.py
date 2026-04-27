import os

from api_client import ApiClient


def _build_api_client() -> ApiClient:
    return ApiClient(base_url=os.getenv("API_BASE_URL", "http://api:5000"))


async def _hello_world_with_client(client: ApiClient) -> dict:
    return await client.get_hello()


async def hello_world() -> dict:
    return await _hello_world_with_client(_build_api_client())
