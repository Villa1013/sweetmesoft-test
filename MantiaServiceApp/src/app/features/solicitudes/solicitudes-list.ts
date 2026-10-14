import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
import {
  ESTADOS_SOLICITUD,
  EstadoSolicitud,
  SolicitudListado,
} from '../../core/models/solicitud.models';
import { CotizacionesService } from '../../core/services/cotizaciones.service';
import { SolicitudesService } from '../../core/services/solicitudes.service';
import { toastAvisoDesdeError, toastError, toastExito } from '../../core/ui/errores';
import { estadoSolicitudLabel } from '../../core/ui/labels';
import { EstadoTag, OrigenBadge, PageHeader, PageShell, SurfaceCard } from '../../shared/ui';

@Component({
  selector: 'app-solicitudes-list',
  imports: [
    RouterLink,
    FormsModule,
    DatePipe,
    ButtonModule,
    SelectModule,
    TableModule,
    TooltipModule,
    ToastModule,
    PageShell,
    PageHeader,
    SurfaceCard,
    EstadoTag,
    OrigenBadge,
  ],
  providers: [MessageService],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './solicitudes-list.html',
})
export class SolicitudesList implements OnInit {
  private readonly solicitudesService = inject(SolicitudesService);
  private readonly cotizacionesService = inject(CotizacionesService);
  private readonly messages = inject(MessageService);

  protected readonly loading = signal(true);
  protected readonly solicitudes = signal<SolicitudListado[]>([]);
  protected readonly filtroEstado = signal<EstadoSolicitud | null>(null);
  protected readonly generando = signal<number | null>(null);

  protected readonly opcionesEstado = [
    { label: 'Todos los estados', value: null as EstadoSolicitud | null },
    ...ESTADOS_SOLICITUD.map((estado) => ({
      label: estadoSolicitudLabel(estado),
      value: estado as EstadoSolicitud | null,
    })),
  ];

  protected readonly filtradas = computed(() => {
    const estado = this.filtroEstado();
    return estado ? this.solicitudes().filter((s) => s.estado === estado) : this.solicitudes();
  });

  ngOnInit(): void {
    this.cargar();
  }

  private cargar(): void {
    this.loading.set(true);
    this.solicitudesService.listar().subscribe({
      next: (data) => {
        this.solicitudes.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.messages.add(toastError(err, 'Error', 'No se pudieron cargar las solicitudes.'));
      },
    });
  }

  onFiltroChange(estado: EstadoSolicitud | null): void {
    this.filtroEstado.set(estado);
  }

  generarCotizacion(solicitud: SolicitudListado): void {
    this.generando.set(solicitud.id);
    this.cotizacionesService.generar(solicitud.id).subscribe({
      next: (cotizacion) => {
        this.generando.set(null);
        this.messages.add(toastExito('Cotización generada', `${cotizacion.numero} creada en borrador.`));
      },
      error: (err) => {
        this.generando.set(null);
        this.messages.add(
          toastAvisoDesdeError(err, 'No se pudo cotizar', 'Revise la información de la solicitud.'),
        );
      },
    });
  }
}
