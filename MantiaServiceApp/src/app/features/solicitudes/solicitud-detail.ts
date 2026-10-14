import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { ESTADOS_SOLICITUD, EstadoSolicitud, SolicitudDetalle } from '../../core/models/solicitud.models';
import { CotizacionesService } from '../../core/services/cotizaciones.service';
import { SolicitudesService } from '../../core/services/solicitudes.service';
import { toastAvisoDesdeError, toastError, toastExito } from '../../core/ui/errores';
import { estadoSolicitudLabel } from '../../core/ui/labels';
import {
  BackLink,
  CardTitle,
  DataItem,
  EstadoTag,
  OrigenBadge,
  PageShell,
  SectionCard,
  SurfaceCard,
} from '../../shared/ui';

@Component({
  selector: 'app-solicitud-detail',
  imports: [
    FormsModule,
    DatePipe,
    ButtonModule,
    SelectModule,
    TableModule,
    TagModule,
    TextareaModule,
    ToastModule,
    TooltipModule,
    PageShell,
    BackLink,
    SurfaceCard,
    SectionCard,
    CardTitle,
    DataItem,
    EstadoTag,
    OrigenBadge,
  ],
  providers: [MessageService],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './solicitud-detail.html',
})
export class SolicitudDetail implements OnInit {
  private readonly solicitudesService = inject(SolicitudesService);
  private readonly cotizacionesService = inject(CotizacionesService);
  private readonly messages = inject(MessageService);
  private readonly router = inject(Router);

  readonly id = input.required<string>();

  protected readonly loading = signal(true);
  protected readonly solicitud = signal<SolicitudDetalle | null>(null);
  protected readonly generando = signal(false);
  protected readonly guardando = signal(false);

  protected estadoEditado: EstadoSolicitud | null = null;
  protected observacionesEditadas = '';

  protected readonly opcionesEstado = ESTADOS_SOLICITUD.map((estado) => ({
    label: estadoSolicitudLabel(estado),
    value: estado as EstadoSolicitud,
  }));

  ngOnInit(): void {
    this.cargar();
  }

  private cargar(): void {
    this.loading.set(true);
    this.solicitudesService.obtener(Number(this.id())).subscribe({
      next: (data) => {
        this.solicitud.set(data);
        this.estadoEditado = data.estado;
        this.observacionesEditadas = data.observaciones;
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.messages.add(toastError(err, 'Error', 'No se pudo cargar la solicitud.'));
      },
    });
  }

  guardarCambios(): void {
    const actual = this.solicitud();
    if (!actual) {
      return;
    }
    this.guardando.set(true);
    this.solicitudesService
      .actualizar(actual.id, {
        estado: this.estadoEditado,
        observaciones: this.observacionesEditadas,
      })
      .subscribe({
        next: (data) => {
          this.solicitud.set(data);
          this.guardando.set(false);
          this.messages.add(toastExito('Guardado', 'La solicitud fue actualizada.'));
        },
        error: (err) => {
          this.guardando.set(false);
          this.messages.add(toastError(err, 'Error', 'No se pudo actualizar la solicitud.'));
        },
      });
  }

  generarCotizacion(): void {
    const actual = this.solicitud();
    if (!actual) {
      return;
    }
    this.generando.set(true);
    this.cotizacionesService.generar(actual.id).subscribe({
      next: (cotizacion) => {
        this.generando.set(false);
        this.router.navigate(['/cotizaciones', cotizacion.id]);
      },
      error: (err) => {
        this.generando.set(false);
        this.messages.add(
          toastAvisoDesdeError(err, 'No se pudo cotizar', 'Revise la información de la solicitud.'),
        );
      },
    });
  }
}
