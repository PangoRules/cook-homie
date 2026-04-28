// src/CookHomie.Web/composables/useToast.ts
export type ToastType = "success" | "error" | "warning" | "info";

export interface Toast {
  id: string;
  type: ToastType;
  message: string;
  createdAt: number;
}

export const useToast = () => {
  const toasts = useState<Toast[]>("toasts", () => []);

  const push = (type: ToastType, message: string) => {
    const id = Math.random().toString(36).slice(2);
    toasts.value.push({ id, type, message, createdAt: Date.now() });
    setTimeout(() => remove(id), 5000);
  };

  const pushSuccess = (message: string) => push("success", message);
  const pushError = (err: Error | string) => {
    const msg = typeof err === "string" ? err : parseErrorMessage(err);
    push("error", msg);
  };
  const pushWarning = (message: string) => push("warning", message);
  const pushInfo = (message: string) => push("info", message);

  const remove = (id: string) => {
    const idx = toasts.value.findIndex((t) => t.id === id);
    if (idx !== -1) toasts.value.splice(idx, 1);
  };

  const clear = () => {
    toasts.value = [];
  };

  return { toasts, pushSuccess, pushError, pushWarning, pushInfo, remove, clear };
};

function parseErrorMessage(err: Error): string {
  const msg = err.message;
  const colonIdx = msg.indexOf(":");
  if (colonIdx !== -1 && colonIdx < msg.length - 1) {
    return msg.slice(colonIdx + 1).trim();
  }
  return msg;
}
