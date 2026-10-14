import { HttpErrorResponse } from '@angular/common/http';

export interface MensajeToast {
  severity: 'success' | 'info' | 'warn' | 'error';
  summary: string;
  detail: string;
}

export function toastExito(summary: string, detail: string): MensajeToast {
  return { severity: 'success', summary, detail };
}

export function toastAviso(summary: string, detail: string): MensajeToast {
  return { severity: 'warn', summary, detail };
}

export function toastError(err: unknown, summary: string, fallback: string): MensajeToast {
  return { severity: 'error', summary, detail: detalleDeError(err) || fallback };
}

export function toastAvisoDesdeError(err: unknown, summary: string, fallback: string): MensajeToast {
  return { severity: 'warn', summary, detail: detalleDeError(err) || fallback };
}

function detalleDeError(err: unknown): string {
  if (err instanceof HttpErrorResponse) {
    const errores = (err.error as { errores?: unknown } | null)?.errores;
    if (Array.isArray(errores) && errores.length > 0) {
      return (errores as string[]).join(' ');
    }
    return err.message;
  }
  return '';
}
