import { Injectable } from '@angular/core';
import * as pdfjsLib from 'pdfjs-dist';

@Injectable({ providedIn: 'root' })
export class PdfService {
  private configurado = false;

  private configurar(): void {
    if (this.configurado) {
      return;
    }
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'pdf.worker.min.mjs';
    this.configurado = true;
  }

  async extraerTexto(file: File): Promise<string> {
    this.configurar();
    const data = new Uint8Array(await file.arrayBuffer());
    const tarea = pdfjsLib.getDocument({ data });
    const documento = await tarea.promise;
    const paginas: string[] = [];

    for (let numero = 1; numero <= documento.numPages; numero++) {
      const pagina = await documento.getPage(numero);
      const contenido = await pagina.getTextContent();
      let textoPagina = '';
      for (const item of contenido.items as Array<{ str?: string; hasEOL?: boolean }>) {
        if (typeof item.str === 'string') {
          textoPagina += item.str;
        }
        if (item.hasEOL) {
          textoPagina += '\n';
        }
      }
      paginas.push(textoPagina);
    }

    await tarea.destroy();
    return paginas
      .join('\n')
      .replace(/[ \t]+/g, ' ')
      .replace(/\n{3,}/g, '\n\n')
      .trim();
  }
}
