import type { DashboardSummary } from "~/types";

export const useDashboard = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<DashboardSummary>(
    "/api/dashboard/summary",
    { pollIntervalMs: 30000 }
  );

  return { data, loading, error, isStale, start, stop, refresh };
};