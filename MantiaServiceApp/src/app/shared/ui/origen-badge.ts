import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { OrigenSolicitud } from '../../core/models/solicitud.models';
import { origenIcon, origenLabel } from '../../core/ui/labels';

@Component({
  selector: 'app-origen-badge',
  imports: [NgClass],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <span class="inline-flex items-center gap-1.5 text-surface-600">
      <i class="pi text-xs" [ngClass]="icono()"></i>
      <span class="text-sm">{{ etiqueta() }}</span>
    </span>
  `,
})
export class OrigenBadge {
  readonly origen = input.required<OrigenSolicitud>();

  protected readonly icono = computed(() => origenIcon(this.origen()));
  protected readonly etiqueta = computed(() => origenLabel(this.origen()));
}
