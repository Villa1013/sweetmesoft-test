import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ActualizarSolicitudRequest,
  AnalizarTextoRequest,
  EstadoSolicitud,
  ResultadoExtraccion,
  SolicitudDetalle,
  SolicitudListado,
} from '../models/solicitud.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SolicitudesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/solicitudes`;

  listar(estado?: EstadoSolicitud): Observable<SolicitudListado[]> {
    let params = new HttpParams();
    if (estado) {
      params = params.set('estado', estado);
    }
    return this.http.get<SolicitudListado[]>(this.baseUrl, { params });
  }

  obtener(id: number): Observable<SolicitudDetalle> {
    return this.http.get<SolicitudDetalle>(`${this.baseUrl}/${id}`);
  }

  analizar(request: AnalizarTextoRequest): Observable<ResultadoExtraccion> {
    return this.http.post<ResultadoExtraccion>(`${this.baseUrl}/analizar`, request);
  }

  registrar(request: AnalizarTextoRequest): Observable<SolicitudDetalle> {
    return this.http.post<SolicitudDetalle>(this.baseUrl, request);
  }

  actualizar(id: number, request: ActualizarSolicitudRequest): Observable<SolicitudDetalle> {
    return this.http.put<SolicitudDetalle>(`${this.baseUrl}/${id}`, request);
  }
}
