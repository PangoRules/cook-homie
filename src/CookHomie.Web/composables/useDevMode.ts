import type { AppEnv } from "@/types";

export const useDevMode = () => {
  const runtimeConfig = useRuntimeConfig();
  const appEnv = runtimeConfig.public?.appEnv as AppEnv | undefined;

  const isDev = appEnv === "development";
  const isStaging = appEnv === "staging";
  const isProd = appEnv === "production";

  return {
    isDev,
    isStaging,
    isProd,
    appEnv,
  };
};
