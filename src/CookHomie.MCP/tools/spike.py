import os

from api_client import ApiClient


async def hello_world(client: ApiClient | None = None) -> dict:
    api_client = client or ApiClient(base_url=os.getenv("API_BASE_URL", "http://api:5000"))
    return await api_client.get_spike_hello()
