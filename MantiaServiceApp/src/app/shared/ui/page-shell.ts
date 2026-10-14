import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

export type AnchoPagina = '4xl' | '5xl' | '6xl' | '7xl';

const ANCHOS: Record<AnchoPagina, string> = {
  '4xl': 'max-w-4xl',
  '5xl': 'max-w-5xl',
  '6xl': 'max-w-6xl',
  '7xl': 'max-w-7xl',
};

const ANCHO_POR_DEFECTO: AnchoPagina = '7xl';

@Component({
  selector: 'app-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<section [class]="clases()"><ng-content /></section>`,
})
export class PageShell {
  readonly maxWidth = input<AnchoPagina | string>(ANCHO_POR_DEFECTO);

  protected readonly clases = computed(() => {
    const ancho = ANCHOS[this.maxWidth() as AnchoPagina] ?? ANCHOS[ANCHO_POR_DEFECTO];
    return `mx-auto ${ancho} px-4 py-6 sm:px-6 lg:px-8`;
  });
}
