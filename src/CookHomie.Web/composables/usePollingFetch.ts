// src/CookHomie.Web/composables/usePollingFetch.ts
export const usePollingFetch = <T>(
  url: string,
  options?: {
    pollIntervalMs?: number;
    onSuccess?: (data: T) => void;
    onError?: (err: Error) => void;
  }
) => {
  const pollIntervalMs = options?.pollIntervalMs ?? 30000;

  const data = useState<T | null>(`poll-${url}`, () => null);
  const loading = useState<boolean>(`poll-loading-${url}`, () => false);
  const hasFetched = useState<boolean>(`poll-fetched-${url}`, () => false);
  const error = useState<string | null>(`poll-error-${url}`, () => null);
  const isStale = useState<boolean>(`poll-stale-${url}`, () => false);

  let intervalId: ReturnType<typeof setInterval> | null = null;
  let started = false;

  const fetchData = async () => {
    if (loading.value && hasFetched.value) return;
    loading.value = true;
    error.value = null;

    try {
      const result = (await $fetch<T>(url)) as T;
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
      hasFetched.value = true;
    }
  };

  const clearPollingInterval = () => {
    if (intervalId) {
      clearInterval(intervalId);
      intervalId = null;
    }
  };

  const handleVisibilityChange = () => {
    if (document.visibilityState === 'visible') {
      // Resume polling if not already running
      if (!intervalId) {
        intervalId = setInterval(fetchData, pollIntervalMs);
      }
    } else {
      // Pause polling when page is hidden
      clearPollingInterval();
    }
  };

  const start = async () => {
    if (started) return;
    started = true;
    await fetchData();
    
    // Add event listener for visibility change
    document.addEventListener('visibilitychange', handleVisibilityChange);
    
    // Start polling immediately 
    intervalId = setInterval(fetchData, pollIntervalMs);
  };

  const stop = () => {
    started = false;
    clearPollingInterval();
    
    // Remove visibility change listener
    document.removeEventListener('visibilitychange', handleVisibilityChange);
  };

  const refresh = async () => {
    await fetchData();
  };

  return { data, loading, error, isStale, start, stop, refresh };
};
