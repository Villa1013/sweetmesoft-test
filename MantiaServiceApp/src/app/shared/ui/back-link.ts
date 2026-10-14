import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-back-link',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <a
      [routerLink]="link()"
      class="fade-in-up mb-4 inline-flex items-center gap-2 text-sm font-medium text-surface-500 hover:text-primary"
    >
      <i class="pi pi-arrow-left"></i> {{ label() }}
    </a>
  `,
})
export class BackLink {
  readonly link = input.required<string | (string | number)[]>();
  readonly label = input.required<string>();
}
