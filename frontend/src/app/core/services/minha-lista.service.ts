import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddToListRequest, UpdateTrackingRequest, UserTituloResponse } from '../models/usuario-titulo.model';
import { StatusTitulo } from '../models/titulo.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MinhaListaService {
  private readonly http = inject(HttpClient);

  adicionar(request: AddToListRequest): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/minha-lista`, request);
  }

  buscarLista(status?: StatusTitulo): Observable<UserTituloResponse[]> {
    let params = new HttpParams();
    if (status) params = params.set('status', status);

    return this.http.get<UserTituloResponse[]>(`${environment.apiUrl}/minha-lista`, { params });
  }

  atualizar(tituloId: string, request: UpdateTrackingRequest): Observable<UserTituloResponse> {
    return this.http.put<UserTituloResponse>(`${environment.apiUrl}/minha-lista/${tituloId}`, request);
  }

  remover(tituloId: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/minha-lista/${tituloId}`);
  }
}