# Task 2: Canonical `formatExpiry` in `utils/date.ts`
**Branch:** `task/task-2-format-expiry-date-util`
**Parent branch:** `feat/milestone2-1-frontend-improvements`
**Parent spec:** `2026-05-03-milestone-2-1-frontend-improvements-design.md`

## Files
- Modify: `src/CookHomie.Web/utils/date.ts`
- Modify: `src/CookHomie.Web/pages/index.vue:103-114` (remove local `formatExpiry`, import shared)
- Modify: `src/CookHomie.Web/pages/inventory.vue:66-74` (remove local `formatExpiry`, import shared)

## Steps

- [ ] **Step 1: Extend `utils/date.ts` with canonical day-diff logic**

Replace the contents of `src/CookHomie.Web/utils/date.ts` with:

```typescript
export type ExpiryState = "expired" | "today" | "tomorrow" | "inNDays";

const startOfLocalDay = (date: Date): Date => {
  const d = new Date(date);
  d.setHours(0, 0, 0, 0);
  return d;
};

export const getExpiryState = (
  expiryDateString: string | null | undefined
): ExpiryState | null => {
  if (!expiryDateString) return null;
  const expiry = new Date(expiryDateString);
  if (isNaN(expiry.getTime())) return null;

  const now = new Date();
  const expiryDay = startOfLocalDay(expiry);
  const todayDay = startOfLocalDay(now);
  const diffDays = Math.round((expiryDay.getTime() - todayDay.getTime()) / (1000 * 60 * 60 * 24));

  if (diffDays < 0) return "expired";
  if (diffDays === 0) return "today";
  if (diffDays === 1) return "tomorrow";
  return "inNDays";
};

export const getExpiryDaysRemaining = (
  expiryDateString: string | null | undefined
): number | null => {
  if (!expiryDateString) return null;
  const expiry = new Date(expiryDateString);
  if (isNaN(expiry.getTime())) return null;
  const now = new Date();
  return Math.round((startOfLocalDay(expiry).getTime() - startOfLocalDay(now).getTime()) / (1000 * 60 * 60 * 24));
};

export const formatExpiryShort = (expiryDateString: string | null | undefined): string => {
  const state = getExpiryState(expiryDateString);
  if (!state) return "No expiry";
  if (state === "expired") return "Expired";
  if (state === "today") return "Expires today";
  if (state === "tomorrow") return "Expires tomorrow";
  const days = getExpiryDaysRemaining(expiryDateString)!;
  return `Expires in ${days}d`;
};

export const formatExpiryLong = (expiryDateString: string | null | undefined): string => {
  const state = getExpiryState(expiryDateString);
  if (!state) return "No expiry date";
  if (state === "expired") {
    const days = getExpiryDaysRemaining(expiryDateString)!;
    return `Expired ${Math.abs(days)} days ago`;
  }
  if (state === "today") return "Expires today";
  if (state === "tomorrow") return "Expires tomorrow";
  const days = getExpiryDaysRemaining(expiryDateString)!;
  return `Expires in ${days} days`;
};

export { formatDate };
```

- [ ] **Step 2: Update `pages/index.vue`**

Remove the local `formatExpiry` function (lines 103–114). Add import at the top of `<script setup>`:

```typescript
import { formatExpiryLong } from "~/utils/date";
```

Replace `formatExpiry(item.expiresAt)` call on line 58 with `formatExpiryLong(item.expiresAt)`.

- [ ] **Step 3: Update `pages/inventory.vue`**

Remove the local `formatExpiry` function (lines 66–74). Add import:

```typescript
import { formatExpiryShort } from "~/utils/date";
```

Replace `formatExpiry(item.expiresAt)` call on line 44 with `formatExpiryShort(item.expiresAt)`.

- [ ] **Step 4: Commit**

```bash
git add src/CookHomie.Web/utils/date.ts src/CookHomie.Web/pages/index.vue src/CookHomie.Web/pages/inventory.vue
git commit -m "feat(web): centralize expiry formatting in utils/date.ts"
```
