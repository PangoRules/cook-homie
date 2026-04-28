export default defineEventHandler(async (_event) => {
  const config = useRuntimeConfig();
  const appEnv = config.public.appEnv;

  return {
    env: appEnv,
    ts: Date.now(),
  };
});
