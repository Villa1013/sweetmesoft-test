import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { CotizacionDetalle } from '../../core/models/cotizacion.models';
import { CotizacionesService } from '../../core/services/cotizaciones.service';
import { toastError, toastExito } from '../../core/ui/errores';
import { CopPipe } from '../../shared/pipes/cop.pipe';
import { BackLink, EstadoTag, PageShell } from '../../shared/ui';

@Component({
  selector: 'app-cotizacion-detail',
  imports: [
    RouterLink,
    DatePipe,
    ButtonModule,
    TableModule,
    TagModule,
    ToastModule,
    TooltipModule,
    CopPipe,
    PageShell,
    BackLink,
    EstadoTag,
  ],
  providers: [MessageService],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './cotizacion-detail.html',
})
export class CotizacionDetail implements OnInit {
  private readonly cotizacionesService = inject(CotizacionesService);
  private readonly messages = inject(MessageService);

  readonly id = input.required<string>();

  protected readonly loading = signal(true);
  protected readonly cotizacion = signal<CotizacionDetalle | null>(null);
  protected readonly enviando = signal(false);

  ngOnInit(): void {
    this.cargar();
  }

  private cargar(): void {
    this.loading.set(true);
    this.cotizacionesService.obtener(Number(this.id())).subscribe({
      next: (data) => {
        this.cotizacion.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.messages.add(toastError(err, 'Error', 'No se pudo cargar la cotización.'));
      },
    });
  }

  marcarEnviada(): void {
    const actual = this.cotizacion();
    if (!actual) {
      return;
    }
    this.enviando.set(true);
    this.cotizacionesService.marcarEnviada(actual.id).subscribe({
      next: (data) => {
        this.cotizacion.set(data);
        this.enviando.set(false);
        this.messages.add(toastExito('Enviada', 'La cotización fue marcada como enviada.'));
      },
      error: (err) => {
        this.enviando.set(false);
        this.messages.add(toastError(err, 'Error', 'No se pudo actualizar la cotización.'));
      },
    });
  }

  imprimir(): void {
    window.print();
  }
}
