import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-card-title',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <h2
      class="text-sm font-semibold uppercase tracking-wide text-surface-500"
      [class.mb-4]="espaciado()"
    >
      {{ title() }}
    </h2>
  `,
})
export class CardTitle {
  readonly title = input.required<string>();
  readonly espaciado = input(true);
}
