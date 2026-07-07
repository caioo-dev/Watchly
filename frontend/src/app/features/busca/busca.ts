import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { TituloService } from '../../core/services/titulo.service';
import {
  TipoTitulo,
  TituloExternoResponse
} from '../../core/models/titulo.model';

@Component({
  selector: 'app-busca',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './busca.html'
})
export class BuscaComponent implements OnInit {
  private readonly tituloService = inject(TituloService);

  readonly query = signal('');
  readonly tipoFiltro = signal<TipoTitulo | ''>('');
  readonly resultados = signal<TituloExternoResponse[]>([]);
  readonly carregando = signal(false);
  readonly erro = signal('');
  readonly buscou = signal(false);
  readonly pagina = signal(1);
  readonly temMais = signal(true);

  ngOnInit(): void {
    if (this.tituloService.voltandoDaBusca) {
      this.tituloService.voltandoDaBusca = false;
      this.restaurarEstado();
      return;
    }

    this.tituloService.limparEstado();
  }

  buscar(pagina = 1): void {
    if (!this.query().trim()) {
      return;
    }

    this.carregando.set(true);
    this.erro.set('');
    this.buscou.set(true);
    this.pagina.set(pagina);

    this.tituloService
      .buscar(
        this.query(),
        this.tipoFiltro() || undefined,
        pagina
      )
      .subscribe({
        next: (resultado) => {
          this.resultados.set(resultado);
          this.temMais.set(resultado.length > 0);
          this.carregando.set(false);

          this.salvarEstado();
        },
        error: () => {
          this.erro.set('Erro ao buscar títulos.');
          this.carregando.set(false);
        }
      });
  }

  limpar(): void {
    this.query.set('');
    this.tipoFiltro.set('');
    this.resultados.set([]);
    this.buscou.set(false);
    this.pagina.set(1);
    this.temMais.set(true);
    this.erro.set('');

    this.tituloService.limparEstado();
  }

  anterior(): void {
    if (this.pagina() > 1) {
      this.buscar(this.pagina() - 1);
    }
  }

  proxima(): void {
    if (this.temMais()) {
      this.buscar(this.pagina() + 1);
    }
  }

  private restaurarEstado(): void {
    const estado = this.tituloService.estadoBusca;

    if (!estado.buscou) {
      return;
    }

    this.query.set(estado.query);
    this.tipoFiltro.set(estado.tipoFiltro);
    this.resultados.set(estado.resultados);
    this.pagina.set(estado.pagina);
    this.buscou.set(estado.buscou);
    this.temMais.set(estado.resultados.length > 0);
  }

  private salvarEstado(): void {
    this.tituloService.estadoBusca = {
      query: this.query(),
      tipoFiltro: this.tipoFiltro(),
      resultados: this.resultados(),
      pagina: this.pagina(),
      buscou: this.buscou()
    };
  }
}