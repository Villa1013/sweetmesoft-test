import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { SkeletonModule } from 'primeng/skeleton';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import {
  ORIGENES_SOLICITUD,
  OrigenSolicitud,
  ResultadoExtraccion,
} from '../../core/models/solicitud.models';
import { PdfService } from '../../core/services/pdf.service';
import { SolicitudesService } from '../../core/services/solicitudes.service';
import { toastAviso, toastError, toastExito } from '../../core/ui/errores';
import {
  BackLink,
  CardTitle,
  DataItem,
  EstadoTag,
  PageHeader,
  PageShell,
  SurfaceCard,
} from '../../shared/ui';

@Component({
  selector: 'app-solicitud-nueva',
  imports: [
    FormsModule,
    ButtonModule,
    SelectModule,
    SkeletonModule,
    TextareaModule,
    ToastModule,
    PageShell,
    BackLink,
    PageHeader,
    SurfaceCard,
    CardTitle,
    DataItem,
    EstadoTag,
  ],
  providers: [MessageService],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './solicitud-nueva.html',
})
export class SolicitudNueva {
  private readonly solicitudesService = inject(SolicitudesService);
  private readonly pdf = inject(PdfService);
  private readonly messages = inject(MessageService);
  private readonly router = inject(Router);

  protected texto = '';
  protected origen: OrigenSolicitud = 'Correo';

  protected readonly analizando = signal(false);
  protected readonly registrando = signal(false);
  protected readonly extrayendo = signal(false);
  protected readonly resultado = signal<ResultadoExtraccion | null>(null);

  protected readonly origenes = ORIGENES_SOLICITUD;

  protected get canalEsPdf(): boolean {
    return this.origen === 'PDF';
  }

  protected get acceptArchivo(): string {
    return this.canalEsPdf ? '.pdf,application/pdf' : '.txt,text/plain';
  }

  protected get etiquetaArchivo(): string {
    return this.canalEsPdf ? 'Cargar PDF' : 'Cargar .txt';
  }

  async onArchivo(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) {
      return;
    }

    this.resultado.set(null);
    const esPdfArchivo =
      file.type === 'application/pdf' || file.name.toLowerCase().endsWith('.pdf');

    if (this.canalEsPdf && !esPdfArchivo) {
      this.messages.add(toastAviso('Formato no válido', 'El canal PDF solo acepta archivos .pdf.'));
      return;
    }
    if (!this.canalEsPdf && esPdfArchivo) {
      this.messages.add(
        toastAviso('Formato no válido', 'El canal seleccionado solo acepta archivos .txt.'),
      );
      return;
    }

    if (!esPdfArchivo) {
      const reader = new FileReader();
      reader.onload = () => {
        this.texto = String(reader.result ?? '');
      };
      reader.readAsText(file);
      return;
    }

    this.extrayendo.set(true);
    try {
      const extraido = await this.pdf.extraerTexto(file);
      this.texto = extraido;
      if (!extraido) {
        this.messages.add(
          toastAviso(
            'PDF sin texto',
            'No se pudo extraer texto del PDF (¿es un documento escaneado?). Pegue el contenido manualmente.',
          ),
        );
      }
    } catch {
      this.messages.add(
        toastError(null, 'Error al leer PDF', 'No se pudo procesar el archivo PDF. Intente nuevamente o pegue el texto.'),
      );
    } finally {
      this.extrayendo.set(false);
    }
  }

  analizar(): void {
    if (this.extrayendo()) {
      return;
    }
    if (!this.texto.trim()) {
      this.messages.add(toastAviso('Texto vacío', 'Pegue o cargue la comunicación del cliente.'));
      return;
    }
    this.analizando.set(true);
    this.resultado.set(null);
    this.solicitudesService.analizar({ texto: this.texto, origen: this.origen }).subscribe({
      next: (data) => {
        this.resultado.set(data);
        this.analizando.set(false);
      },
      error: (err) => {
        this.analizando.set(false);
        this.messages.add(toastError(err, 'Error', 'No se pudo analizar el texto.'));
      },
    });
  }

  registrar(): void {
    this.registrando.set(true);
    this.solicitudesService.registrar({ texto: this.texto, origen: this.origen }).subscribe({
      next: (solicitud) => {
        this.registrando.set(false);
        this.messages.add(
          toastExito('Solicitud registrada', `${solicitud.numero} creada correctamente.`),
        );
        this.router.navigate(['/solicitudes', solicitud.id]);
      },
      error: (err) => {
        this.registrando.set(false);
        this.messages.add(toastError(err, 'Error', 'No se pudo registrar la solicitud.'));
      },
    });
  }
}
