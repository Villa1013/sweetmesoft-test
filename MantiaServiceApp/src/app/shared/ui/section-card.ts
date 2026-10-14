import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-section-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="fade-in-up rounded-2xl border border-surface-200 bg-white shadow-sm">
      <div class="flex items-center justify-between border-b border-surface-100 px-5 py-4">
        <h2 class="font-semibold text-surface-900">{{ title() }}</h2>
        <ng-content select="[acciones]" />
      </div>
      <ng-content />
    </section>
  `,
})
export class SectionCard {
  readonly title = input.required<string>();
}
