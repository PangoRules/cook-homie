#!/usr/bin/env bash

set -euo pipefail

# Skip spike verification if ENABLE_SPIKE_CHECK is not set to "1"
if [[ "${ENABLE_SPIKE_CHECK:-0}" != "1" ]]; then
  echo "Spike verification disabled by ENABLE_SPIKE_CHECK env var. Skipping..."
  exit 0
fi

API_PORT="${API_PORT:-5000}"
WEB_PORT="${WEB_PORT:-3000}"

if ! command -v curl >/dev/null 2>&1; then
  printf '[FAIL] curl is required for spike verification. Install curl and retry.\n'
  exit 2
fi

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

failed_checks=0

if ! check_endpoint "API spike endpoint" "http://localhost:${API_PORT}/spike/hello" "Hello from C#"; then
  failed_checks=$((failed_checks + 1))
fi

if ! check_endpoint "Web spike proxy" "http://localhost:${WEB_PORT}/api/spike/hello" "Hello from C#"; then
  failed_checks=$((failed_checks + 1))
fi

if [[ "$failed_checks" -gt 0 ]]; then
  printf 'Spike verification failed (%s endpoint check(s) failed).\n' "$failed_checks"
  exit 1
fi

echo "Spike verification complete."
