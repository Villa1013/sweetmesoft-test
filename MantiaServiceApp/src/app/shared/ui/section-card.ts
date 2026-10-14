import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-section-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="fade-in-up rounded-2xl border border-line bg-panel shadow-sm">
      <div class="flex items-center justify-between border-b border-line px-5 py-4">
        <h2 class="font-semibold text-body">{{ title() }}</h2>
        <ng-content select="[acciones]" />
      </div>
      <ng-content />
    </section>
  `,
})
export class SectionCard {
  readonly title = input.required<string>();
}
