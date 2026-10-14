import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <footer
      class="border-t border-surface-200 px-4 py-4 text-center text-xs text-surface-400 sm:px-6 lg:px-8"
    >
      © 2026 <b>SweetMeSoft</b>. Todos los derechos reservados.
    </footer>
  `,
})
export class Footer {}
