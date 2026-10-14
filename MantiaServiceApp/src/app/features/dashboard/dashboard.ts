import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { forkJoin } from 'rxjs';
import { CotizacionListado } from '../../core/models/cotizacion.models';
import { SolicitudListado } from '../../core/models/solicitud.models';
import { CotizacionesService } from '../../core/services/cotizaciones.service';
import { SolicitudesService } from '../../core/services/solicitudes.service';
import { origenIcon, origenLabel } from '../../core/ui/labels';
import { CopPipe } from '../../shared/pipes/cop.pipe';
import { EstadoTag, PageHeader, PageShell, SectionCard, StatCard } from '../../shared/ui';

@Component({
  selector: 'app-dashboard',
  imports: [
    RouterLink,
    NgClass,
    ButtonModule,
    SkeletonModule,
    CopPipe,
    PageShell,
    PageHeader,
    StatCard,
    EstadoTag,
    SectionCard,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit {
  private readonly solicitudesService = inject(SolicitudesService);
  private readonly cotizacionesService = inject(CotizacionesService);

  protected readonly loading = signal(true);
  protected readonly solicitudes = signal<SolicitudListado[]>([]);
  protected readonly cotizaciones = signal<CotizacionListado[]>([]);

  protected readonly listas = computed(
    () => this.solicitudes().filter((s) => s.estado === 'ListaParaCotizar').length,
  );
  protected readonly requierenAccion = computed(
    () => this.solicitudes().filter((s) => s.estado !== 'ListaParaCotizar').length,
  );
  protected readonly valorTotal = computed(() =>
    this.cotizaciones().reduce((acc, c) => acc + c.total, 0),
  );
  protected readonly recientes = computed(() =>
    [...this.solicitudes()]
      .sort((a, b) => b.fechaRecepcion.localeCompare(a.fechaRecepcion))
      .slice(0, 5),
  );

  protected readonly origenLabel = origenLabel;
  protected readonly origenIcon = origenIcon;

  ngOnInit(): void {
    forkJoin({
      solicitudes: this.solicitudesService.listar(),
      cotizaciones: this.cotizacionesService.listar(),
    }).subscribe({
      next: ({ solicitudes, cotizaciones }) => {
        this.solicitudes.set(solicitudes);
        this.cotizaciones.set(cotizaciones);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
