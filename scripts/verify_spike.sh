#!/usr/bin/env bash

set -euo pipefail

API_PORT="${API_PORT:-5000}"
WEB_PORT="${WEB_PORT:-3000}"

check_endpoint() {
  local name="$1"
  local url="$2"
  local expected_fragment="$3"

  local response
  if ! response="$(curl --silent --show-error --fail --max-time 5 "$url")"; then
    echo "[FAIL] $name unreachable at $url"
    return 1
  fi

  if [[ "$response" != *"$expected_fragment"* ]]; then
    echo "[FAIL] $name response missing expected text '$expected_fragment'"
    echo "       response: $response"
    return 1
  fi

  echo "[PASS] $name returned expected payload"
}

check_endpoint "API spike endpoint" "http://localhost:${API_PORT}/spike/hello" "Hello from C#"
check_endpoint "Web spike proxy" "http://localhost:${WEB_PORT}/api/spike/hello" "Hello from C#"

echo "Spike verification complete."
