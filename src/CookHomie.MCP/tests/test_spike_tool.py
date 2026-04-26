import pytest

from tools.spike import hello_world


@pytest.mark.asyncio
async def test_hello_world_returns_exact_payload_with_injected_client() -> None:
    class FakeApiClient:
        async def get_spike_hello(self) -> dict[str, str]:
            return {"message": "hello"}

    payload = await hello_world(client=FakeApiClient())

    assert payload == {"message": "hello"}


@pytest.mark.asyncio
async def test_hello_world_uses_api_base_url_env_var(monkeypatch: pytest.MonkeyPatch) -> None:
    observed: dict[str, str] = {}

    class FakeApiClient:
        def __init__(self, base_url: str) -> None:
            observed["base_url"] = base_url

        async def get_spike_hello(self) -> dict[str, str]:
            return {"message": "hello"}

    monkeypatch.setenv("API_BASE_URL", "http://localhost:5050")
    monkeypatch.setattr("tools.spike.ApiClient", FakeApiClient)

    payload = await hello_world()

    assert observed == {"base_url": "http://localhost:5050"}
    assert payload == {"message": "hello"}


@pytest.mark.asyncio
async def test_hello_world_uses_default_api_base_url_when_unset(monkeypatch: pytest.MonkeyPatch) -> None:
    observed: dict[str, str] = {}

    class FakeApiClient:
        def __init__(self, base_url: str) -> None:
            observed["base_url"] = base_url

        async def get_spike_hello(self) -> dict[str, str]:
            return {"message": "hello"}

    monkeypatch.delenv("API_BASE_URL", raising=False)
    monkeypatch.setattr("tools.spike.ApiClient", FakeApiClient)

    payload = await hello_world()

    assert observed == {"base_url": "http://api:5000"}
    assert payload == {"message": "hello"}
