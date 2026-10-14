import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { SkeletonModule } from 'primeng/skeleton';
import { CountUpDirective } from '../directives/count-up.directive';

export type TemaIcono = 'primary' | 'verde' | 'ambar' | 'claro';

const TEMAS: Record<TemaIcono, string> = {
  primary: 'bg-primary-50 text-primary dark:bg-primary-950/40 dark:text-primary-300',
  verde: 'bg-green-50 text-green-600 dark:bg-green-950/40 dark:text-green-400',
  ambar: 'bg-amber-50 text-amber-600 dark:bg-amber-950/40 dark:text-amber-400',
  claro: 'bg-white/20',
};

@Component({
  selector: 'app-stat-card',
  imports: [NgClass, SkeletonModule, CountUpDirective],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <article class="fade-in-up rounded-2xl border p-5 shadow-sm" [class]="claseTarjeta()">
      <div class="flex items-center justify-between">
        <span class="grid h-10 w-10 place-items-center rounded-xl" [class]="claseTemaIcono()">
          <i class="pi" [ngClass]="icono()"></i>
        </span>
        @if (destacada()) {
          <i class="pi pi-arrow-up-right text-body-muted"></i>
        }
      </div>

      @if (cargando()) {
        <p-skeleton
          [width]="formato() === 'cop' ? '7rem' : '4rem'"
          height="2rem"
          styleClass="mt-4"
        />
      } @else {
        <p
          class="mt-4 font-extrabold tracking-tight"
          [class]="claseValor()"
          [appCountUp]="valor()"
          [countUpFormat]="formato()"
        ></p>
      }

      <p class="mt-1 text-sm" [class]="claseEtiqueta()">{{ etiqueta() }}</p>
    </article>
  `,
})
export class StatCard {
  readonly icono = input.required<string>();
  readonly etiqueta = input.required<string>();
  readonly valor = input.required<number>();
  readonly cargando = input(false);
  readonly formato = input<'number' | 'cop'>('number');
  readonly destacada = input(false);
  readonly invertida = input(false);
  readonly tema = input<TemaIcono>('primary');

  protected readonly claseTemaIcono = computed(() => TEMAS[this.tema()]);
  protected readonly claseTarjeta = computed(() =>
    this.invertida()
      ? 'border-primary bg-primary text-primary-contrast shadow-md'
      : 'border-line bg-panel',
  );
  protected readonly claseValor = computed(() => (this.invertida() ? 'text-2xl' : 'text-3xl'));
  protected readonly claseEtiqueta = computed(() =>
    this.invertida() ? 'opacity-80' : 'text-body-muted',
  );
}
