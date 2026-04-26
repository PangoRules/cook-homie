import httpx
import os


class ApiClient:
    def __init__(self, base_url: str | None = None) -> None:
        default_url = os.environ.get("API_BASE_URL") or os.environ.get("COOKHOMIE_API_BASE_URL") or "http://localhost:5000"
        self.base_url = (base_url or default_url).rstrip("/")

    async def get_inventory(self, location: str | None = None) -> list[dict]:
        params = {"location": location} if location else None
        async with httpx.AsyncClient(base_url=self.base_url, timeout=10.0) as client:
            response = await client.get("/api/inventory", params=params)
            response.raise_for_status()
            return response.json()
