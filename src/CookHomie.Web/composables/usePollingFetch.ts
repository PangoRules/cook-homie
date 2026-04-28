// src/CookHomie.Web/composables/usePollingFetch.ts
export const usePollingFetch = <T>(url: string, options?: {
  pollIntervalMs?: number;
  onSuccess?: (data: T) => void;
  onError?: (err: Error) => void;
}) => {
  const pollIntervalMs = options?.pollIntervalMs ?? 30000;

  const data = useState<T | null>(`poll-${url}`, () => null);
  const loading = useState<boolean>(`poll-loading-${url}`, () => false);
  const error = useState<string | null>(`poll-error-${url}`, () => null);
  const isStale = useState<boolean>(`poll-stale-${url}`, () => false);

  let intervalId: ReturnType<typeof setInterval> | null = null;

  const fetchData = async () => {
    if (loading.value) return;
    loading.value = true;
    error.value = null;

    try {
      const result = await $fetch<T>(url);
      data.value = result;
      isStale.value = false;
      options?.onSuccess?.(result);
    } catch (err) {
      const msg = err instanceof Error ? err.message : "Fetch failed";
      error.value = msg;
      isStale.value = data.value !== null;
      options?.onError?.(err instanceof Error ? err : new Error(msg));
    } finally {
      loading.value = false;
    }
  };

  const start = async () => {
    await fetchData();
    intervalId = setInterval(fetchData, pollIntervalMs);
  };

  const stop = () => {
    if (intervalId !== null) {
      clearInterval(intervalId);
      intervalId = null;
    }
  };

  const refresh = async () => {
    await fetchData();
  };

  return { data, loading, error, isStale, start, stop, refresh };
};