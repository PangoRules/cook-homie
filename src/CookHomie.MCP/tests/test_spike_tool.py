import pytest
import sys
from pathlib import Path

sys.path.append(str(Path(__file__).resolve().parents[1]))

from tools.spike import hello_world


@pytest.mark.asyncio
async def test_hello_world_returns_message_payload(monkeypatch: pytest.MonkeyPatch) -> None:
    class FakeApiClient:
        def __init__(self, base_url: str) -> None:
            self.base_url = base_url

        async def get_spike_hello(self) -> dict[str, str]:
            return {"message": "hello"}

    monkeypatch.setattr("tools.spike.ApiClient", FakeApiClient)

    payload = await hello_world()

    assert "message" in payload
