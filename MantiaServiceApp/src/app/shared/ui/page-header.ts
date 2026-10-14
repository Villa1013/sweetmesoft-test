import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-page-header',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <header class="fade-in-up flex flex-wrap items-end justify-between gap-4">
      <div>
        @if (eyebrow()) {
          <p class="text-sm font-medium text-body-muted">{{ eyebrow() }}</p>
        }
        <h1 class="text-2xl font-extrabold tracking-tight text-body">{{ title() }}</h1>
        @if (subtitle()) {
          <p class="mt-1 text-sm text-body-muted">{{ subtitle() }}</p>
        }
      </div>
      <ng-content />
    </header>
  `,
})
export class PageHeader {
  readonly title = input.required<string>();
  readonly subtitle = input('');
  readonly eyebrow = input('');
}
