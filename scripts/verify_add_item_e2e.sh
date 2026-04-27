#!/usr/bin/env bash
set -euo pipefail

# End-to-end verification script for the add-item vertical slice
# Tests POST /api/inventory and GET /api/inventory endpoints

echo "Starting end-to-end verification of add-item vertical slice..."

# Test data for the inventory item
ITEM_NAME="Oat Milk"
ITEM_CATEGORY="Produce"
ITEM_QUANTITY=5.0
ITEM_UNIT="units"
ITEM_LOCATION="Pantry"

# API base URL (assuming local development)
API_BASE_URL="http://localhost:5000"

# Test 1: POST to add a new inventory item
echo "Test 1: Adding new inventory item..."
POST_RESPONSE=$(curl -s -w "%{http_code}" -X POST "$API_BASE_URL/api/inventory" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "'$ITEM_NAME'",
    "category": "'$ITEM_CATEGORY'",
    "quantity": '$ITEM_QUANTITY',
    "unit": "'$ITEM_UNIT'",
    "location": "'$ITEM_LOCATION'",
    "expiresAt": null,
    "isOpen": false,
    "notes": "Test item for verification"
  }')

# Extract HTTP status code from curl response
POST_STATUS_CODE=${POST_RESPONSE: -3}
POST_BODY=${POST_RESPONSE:0:-3}

# Verify POST request was successful
if [ "$POST_STATUS_CODE" != "201" ]; then
  echo "❌ POST request failed with status: $POST_STATUS_CODE"
  echo "Response body: $POST_BODY"
  exit 1
fi

# Verify returned JSON is valid
if ! echo "$POST_BODY" | jq empty >/dev/null 2>&1; then
  echo "❌ POST response is not valid JSON"
  echo "Response body: $POST_BODY"
  exit 1
fi

echo "✅ POST request successful with status 201"

# Extract the new item's ID for the next test
ITEM_ID=$(echo "$POST_BODY" | jq -r '.id')

# Test 2: GET all inventory items to verify the item was added
echo "Test 2: Retrieving all inventory items..."
GET_RESPONSE=$(curl -s -w "%{http_code}" -X GET "$API_BASE_URL/api/inventory")

# Extract HTTP status code from curl response
GET_STATUS_CODE=${GET_RESPONSE: -3}
GET_BODY=${GET_RESPONSE:0:-3}

# Verify GET request was successful
if [ "$GET_STATUS_CODE" != "200" ]; then
  echo "❌ GET request failed with status: $GET_STATUS_CODE"
  echo "Response body: $GET_BODY"
  exit 1
fi

# Verify returned JSON is valid
if ! echo "$GET_BODY" | jq empty >/dev/null 2>&1; then
  echo "❌ GET response is not valid JSON"
  echo "Response body: $GET_BODY"
  exit 1
fi

echo "✅ GET request successful with status 200"

# Test 3: Verify the added item is present in the GET response
echo "Test 3: Verifying added item is present in GET response..."
if echo "$GET_BODY" | jq -e ".[] | select(.id == \"$ITEM_ID\")" >/dev/null 2>&1; then
    echo "✅ Added item with ID $ITEM_ID found in GET response"
else
    echo "❌ Added item with ID $ITEM_ID NOT found in GET response"
    echo "Full GET response: $GET_BODY"
    exit 1
fi

# Test 4: Verify item details match what was added
echo "Test 4: Verifying item details..."
ITEM_FROM_GET=$(echo "$GET_BODY" | jq -r ".[] | select(.id == \"$ITEM_ID\")")
ITEM_NAME_FROM_GET=$(echo "$ITEM_FROM_GET" | jq -r '.name')

if [ "$ITEM_NAME_FROM_GET" == "$ITEM_NAME" ]; then
    echo "✅ Item name matches: $ITEM_NAME"
else
    echo "❌ Item name mismatch. Expected: $ITEM_NAME, Got: $ITEM_NAME_FROM_GET"
    exit 1
fi

echo "🎉 All tests passed! The add-item vertical slice is working correctly."
