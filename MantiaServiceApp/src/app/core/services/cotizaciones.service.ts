import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CotizacionDetalle, CotizacionListado } from '../models/cotizacion.models';

@Injectable({ providedIn: 'root' })
export class CotizacionesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/cotizaciones';

  listar(solicitudId?: number): Observable<CotizacionListado[]> {
    let params = new HttpParams();
    if (solicitudId != null) {
      params = params.set('solicitudId', solicitudId);
    }
    return this.http.get<CotizacionListado[]>(this.baseUrl, { params });
  }

  obtener(id: number): Observable<CotizacionDetalle> {
    return this.http.get<CotizacionDetalle>(`${this.baseUrl}/${id}`);
  }

  generar(solicitudId: number): Observable<CotizacionDetalle> {
    return this.http.post<CotizacionDetalle>(`${this.baseUrl}/solicitudes/${solicitudId}`, {});
  }

  marcarEnviada(id: number): Observable<CotizacionDetalle> {
    return this.http.post<CotizacionDetalle>(`${this.baseUrl}/${id}/marcar-enviada`, {});
  }
}
