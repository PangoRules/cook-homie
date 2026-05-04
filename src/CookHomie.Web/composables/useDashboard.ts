import type { DashboardSummary } from "~/types";
import { API_ROUTES } from "~/utils/apiRoutes";

export const useDashboard = () => {
  const { data, loading, error, isStale, start, stop, refresh } = usePollingFetch<DashboardSummary>(
    API_ROUTES.DASHBOARD.SUMMARY,
    { pollIntervalMs: 30000 }
  );

  return { data, loading, error, isStale, start, stop, refresh };
};
