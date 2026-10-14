import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { MessageService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { Tarifa } from '../../core/models/tarifa.models';
import { TarifasService } from '../../core/services/tarifas.service';
import { toastError } from '../../core/ui/errores';
import { tipoTarifaLabel } from '../../core/ui/labels';
import { CopPipe } from '../../shared/pipes/cop.pipe';
import { PageHeader, PageShell, SurfaceCard } from '../../shared/ui';

@Component({
  selector: 'app-tarifas',
  imports: [TableModule, TagModule, ToastModule, CopPipe, PageShell, PageHeader, SurfaceCard],
  providers: [MessageService],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './tarifas.html',
})
export class Tarifas implements OnInit {
  private readonly tarifasService = inject(TarifasService);
  private readonly messages = inject(MessageService);

  protected readonly loading = signal(true);
  protected readonly tarifas = signal<Tarifa[]>([]);
  protected readonly versionActiva = signal<string | null>(null);

  protected readonly versiones = computed(() =>
    [...new Set(this.tarifas().map((t) => t.version))].sort(),
  );

  protected readonly filtradas = computed(() => {
    const version = this.versionActiva();
    return version ? this.tarifas().filter((t) => t.version === version) : this.tarifas();
  });

  protected readonly tipoTarifaLabel = tipoTarifaLabel;

  ngOnInit(): void {
    this.tarifasService.listar().subscribe({
      next: (data) => {
        this.tarifas.set(data);
        this.versionActiva.set(this.versiones()[this.versiones().length - 1] ?? null);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.messages.add(toastError(err, 'Error', 'No se pudieron cargar las tarifas.'));
      },
    });
  }

  seleccionarVersion(version: string | null): void {
    this.versionActiva.set(version);
  }
}
