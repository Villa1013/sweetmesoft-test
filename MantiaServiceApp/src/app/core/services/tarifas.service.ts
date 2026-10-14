import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Tarifa } from '../models/tarifa.models';

@Injectable({ providedIn: 'root' })
export class TarifasService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/tarifas';

  listar(version?: string): Observable<Tarifa[]> {
    let params = new HttpParams();
    if (version) {
      params = params.set('version', version);
    }
    return this.http.get<Tarifa[]>(this.baseUrl, { params });
  }
}
