import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-data-item',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div>
      <dt class="text-xs font-medium uppercase tracking-wide text-surface-400">{{ label() }}</dt>
      <dd class="mt-1 text-sm font-medium text-surface-800">{{ value() || '—' }}</dd>
    </div>
  `,
})
export class DataItem {
  readonly label = input.required<string>();
  readonly value = input<string | null | undefined>('');
}
