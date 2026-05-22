import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { TituloDetalheResponse, TituloExternoResponse, TipoTitulo, FonteTitulo } from '../models/titulo.model';
import { environment } from '../../../environments/environment';

export interface EstadoBusca {
  busca: string;
  tipoFiltro: TipoTitulo | '';
  resultados: TituloExternoResponse[];
  pagina: number;
  buscou: boolean;
}

const ESTADO_INICIAL: EstadoBusca = {
  busca:      '',
  tipoFiltro: '',
  resultados: [],
  pagina:     1,
  buscou:     false
};

@Injectable({ providedIn: 'root' })
export class TituloService {
  private readonly http = inject(HttpClient);

  estadoBusca: EstadoBusca = { ...ESTADO_INICIAL };
  voltandoDaBusca = false;

  buscar(busca: string, tipo?: TipoTitulo, page: number = 1): Observable<TituloExternoResponse[]> {
    const params = this.montarParamsBusca(busca, tipo, page);

    return this.http.get<TituloExternoResponse[]>(`${environment.apiUrl}/titulos`, { params }).pipe(
      tap(resultados => this.salvarEstado(busca, tipo, resultados, page))
    );
  }

  buscarDetalhe(fonte: FonteTitulo, tipo: TipoTitulo, externalId: string): Observable<TituloDetalheResponse> {
    return this.http.get<TituloDetalheResponse>(`${environment.apiUrl}/titulos/${fonte}/${tipo}/${externalId}`);
  }

  limparEstado(): void {
    this.estadoBusca = { ...ESTADO_INICIAL };
  }

  // ──────────────────────────────────────────────
  // Helpers
  // ──────────────────────────────────────────────

  private montarParamsBusca(busca: string, tipo?: TipoTitulo, page: number = 1): HttpParams {
    let params = new HttpParams().set('busca', busca).set('page', page);
    if (tipo) params = params.set('tipo', tipo);
    return params;
  }

  private salvarEstado(
    busca: string, tipo: TipoTitulo | undefined,
    resultados: TituloExternoResponse[], pagina: number): void
  {
    this.estadoBusca = {
      busca,
      tipoFiltro: tipo ?? '',
      resultados,
      pagina,
      buscou: true
    };
  }
}