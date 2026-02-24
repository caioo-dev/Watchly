import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TituloDetalheResponse, TituloExternoResponse, TipoTitulo, FonteTitulo } from '../models/titulo.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TituloService {
  private readonly http = inject(HttpClient);

  buscar(query: string, tipo?: TipoTitulo): Observable<TituloExternoResponse[]> {
    let params = new HttpParams().set('query', query);
    if (tipo) params = params.set('tipo', tipo);

    return this.http.get<TituloExternoResponse[]>(`${environment.apiUrl}/titulos`, { params });
  }

  buscarDetalhe(fonte: FonteTitulo, externalId: string): Observable<TituloDetalheResponse> {
    return this.http.get<TituloDetalheResponse>(`${environment.apiUrl}/titulos/${fonte}/${externalId}`);
  }
}