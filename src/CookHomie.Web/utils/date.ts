export type ExpiryState = "expired" | "today" | "tomorrow" | "inNDays";

const startOfLocalDay = (date: Date): Date => {
  const d = new Date(date);
  d.setHours(0, 0, 0, 0);
  return d;
};

export const getExpiryState = (expiryDateString: string | null | undefined): ExpiryState | null => {
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
  return Math.round(
    (startOfLocalDay(expiry).getTime() - startOfLocalDay(now).getTime()) / (1000 * 60 * 60 * 24)
  );
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
