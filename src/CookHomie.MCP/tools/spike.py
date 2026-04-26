from api_client import ApiClient


async def hello_world() -> dict:
    client = ApiClient(base_url="http://api:5000")
    return await client.get_spike_hello()
