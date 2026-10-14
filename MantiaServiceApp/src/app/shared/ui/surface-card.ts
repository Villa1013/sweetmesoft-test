import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div [class]="clases()"><ng-content /></div>`,
})
export class SurfaceCard {
  readonly padded = input(true);

  protected readonly clases = computed(
    () =>
      `rounded-2xl border border-surface-200 bg-white shadow-sm${this.padded() ? ' p-5' : ''}`,
  );
}
