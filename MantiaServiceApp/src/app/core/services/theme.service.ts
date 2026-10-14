import { DOCUMENT } from '@angular/common';
import { Injectable, computed, effect, inject, signal } from '@angular/core';

export type Tema = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private static readonly CLAVE = 'mantia-theme';
  private readonly documento = inject(DOCUMENT);

  private readonly temaActual = signal<Tema>(ThemeService.temaInicial());
  readonly tema = this.temaActual.asReadonly();
  readonly oscuro = computed(() => this.temaActual() === 'dark');

  constructor() {
    effect(() => this.aplicar(this.temaActual()));
  }

  alternar(): void {
    this.establecer(this.oscuro() ? 'light' : 'dark');
  }

  establecer(tema: Tema): void {
    this.temaActual.set(tema);
    try {
      localStorage.setItem(ThemeService.CLAVE, tema);
    } catch {
      /* almacenamiento no disponible */
    }
  }

  private aplicar(tema: Tema): void {
    const raiz = this.documento.documentElement;
    raiz.classList.toggle('p-dark', tema === 'dark');
    raiz.style.colorScheme = tema;
  }

  private static temaInicial(): Tema {
    try {
      const guardado = localStorage.getItem(ThemeService.CLAVE);
      if (guardado === 'dark' || guardado === 'light') {
        return guardado;
      }
    } catch {
      /* almacenamiento no disponible */
    }
    const prefiereOscuro = window.matchMedia?.('(prefers-color-scheme: dark)').matches;
    return prefiereOscuro ? 'dark' : 'light';
  }
}
