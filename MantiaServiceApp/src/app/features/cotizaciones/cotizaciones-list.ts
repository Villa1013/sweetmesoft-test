import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import {
  ESTADOS_COTIZACION,
  CotizacionListado,
  EstadoCotizacion,
} from '../../core/models/cotizacion.models';
import { CotizacionesService } from '../../core/services/cotizaciones.service';
import { toastError, toastExito } from '../../core/ui/errores';
import { estadoCotizacionLabel } from '../../core/ui/labels';
import { CopPipe } from '../../shared/pipes/cop.pipe';
import { EstadoTag, PageHeader, PageShell, SurfaceCard } from '../../shared/ui';

@Component({
  selector: 'app-cotizaciones-list',
  imports: [
    RouterLink,
    FormsModule,
    DatePipe,
    ButtonModule,
    SelectModule,
    TableModule,
    TagModule,
    ToastModule,
    TooltipModule,
    CopPipe,
    PageShell,
    PageHeader,
    SurfaceCard,
    EstadoTag,
  ],
  providers: [MessageService],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './cotizaciones-list.html',
})
export class CotizacionesList implements OnInit {
  private readonly cotizacionesService = inject(CotizacionesService);
  private readonly messages = inject(MessageService);

  protected readonly loading = signal(true);
  protected readonly cotizaciones = signal<CotizacionListado[]>([]);
  protected readonly filtroEstado = signal<EstadoCotizacion | null>(null);
  protected readonly enviando = signal<number | null>(null);

  protected readonly opcionesEstado = [
    { label: 'Todos los estados', value: null as EstadoCotizacion | null },
    ...ESTADOS_COTIZACION.map((estado) => ({
      label: estadoCotizacionLabel(estado),
      value: estado as EstadoCotizacion | null,
    })),
  ];

  protected readonly filtradas = computed(() => {
    const estado = this.filtroEstado();
    return estado
      ? this.cotizaciones().filter((c) => c.estado === estado)
      : this.cotizaciones();
  });

  protected readonly totalVisible = computed(() =>
    this.filtradas()
      .filter((c) => c.estado !== 'Rechazada')
      .reduce((acc, c) => acc + c.total, 0),
  );

  ngOnInit(): void {
    this.cargar();
  }

  private cargar(): void {
    this.loading.set(true);
    this.cotizacionesService.listar().subscribe({
      next: (data) => {
        this.cotizaciones.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.messages.add(toastError(err, 'Error', 'No se pudieron cargar las cotizaciones.'));
      },
    });
  }

  onFiltroChange(estado: EstadoCotizacion | null): void {
    this.filtroEstado.set(estado);
  }

  marcarEnviada(c: CotizacionListado): void {
    this.enviando.set(c.id);
    this.cotizacionesService.marcarEnviada(c.id).subscribe({
      next: (actualizada) => {
        this.enviando.set(null);
        this.cotizaciones.update((list) =>
          list.map((item) => (item.id === actualizada.id ? { ...item, estado: actualizada.estado } : item)),
        );
        this.messages.add(
          toastExito('Cotización enviada', `${actualizada.numero} marcada como enviada.`),
        );
      },
      error: (err) => {
        this.enviando.set(null);
        this.messages.add(toastError(err, 'Error', 'No se pudo actualizar la cotización.'));
      },
    });
  }
}
