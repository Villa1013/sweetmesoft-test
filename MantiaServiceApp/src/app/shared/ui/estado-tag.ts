import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { TagModule } from 'primeng/tag';
import { EstadoCotizacion } from '../../core/models/cotizacion.models';
import { EstadoSolicitud } from '../../core/models/solicitud.models';
import {
  estadoCotizacionLabel,
  estadoCotizacionSeverity,
  estadoSolicitudLabel,
  estadoSolicitudSeverity,
  TagSeverity,
} from '../../core/ui/labels';

export type TipoEstado = 'solicitud' | 'cotizacion';

@Component({
  selector: 'app-estado-tag',
  imports: [TagModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<p-tag [value]="etiqueta()" [severity]="severidad()" />`,
})
export class EstadoTag {
  readonly tipo = input.required<TipoEstado>();
  readonly estado = input.required<EstadoSolicitud | EstadoCotizacion>();

  protected readonly etiqueta = computed(() =>
    this.tipo() === 'solicitud'
      ? estadoSolicitudLabel(this.estado() as EstadoSolicitud)
      : estadoCotizacionLabel(this.estado() as EstadoCotizacion),
  );

  protected readonly severidad = computed<TagSeverity>(() =>
    this.tipo() === 'solicitud'
      ? estadoSolicitudSeverity(this.estado() as EstadoSolicitud)
      : estadoCotizacionSeverity(this.estado() as EstadoCotizacion),
  );
}
