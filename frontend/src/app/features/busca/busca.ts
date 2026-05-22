import { Component, OnInit, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TituloService } from '../../core/services/titulo.service';
import { TituloExternoResponse, TipoTitulo } from '../../core/models/titulo.model';

@Component({
  selector: 'app-busca',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './busca.html'
})
export class BuscaComponent implements OnInit {
  private readonly tituloService = inject(TituloService);
  private readonly router        = inject(Router);
  // state com signals
  query      = signal('');
  tipoFiltro = signal<TipoTitulo | ''>('');
  resultados = signal<TituloExternoResponse[]>([]);
  carregando = signal(false);
  erro       = signal('');
  buscou     = signal(false);
  pagina     = signal(1);
  temMais    = signal(true);
  temMais    = true;

  ngOnInit(): void {
    if (this.tituloService.voltandoDaBusca) {
      this.tituloService.voltandoDaBusca = false;
      this.restaurarEstado();
    } else {
      this.tituloService.limparEstado();
    }
  }

    this.query.set('');
    this.tipoFiltro.set('');
    this.resultados.set([]);
    this.buscou.set(false);
    this.pagina.set(1);
    this.temMais.set(true);
    this.erro.set('');

    this.erro       = '';
    this.tituloService.limparEstado();
  }

    if (!this.query().trim()) return;
    if (!this.query.trim()) return;

    this.carregando.set(true);
    this.erro.set('');
    this.buscou.set(true);
    this.pagina.set(pagina);

    this.tituloService
      .buscar(this.query(), this.tipoFiltro() || undefined, pagina)
      .subscribe({
        next: (data) => {
          this.resultados.set(data);
          this.temMais.set(data.length > 0);
          this.carregando.set(false);
          this.salvarEstado();
        },
        error: () => {
          this.erro.set('Erro ao buscar títulos.');
          this.carregando.set(false);
        }
      });
    });
  }

  anterior(): void {
    if (this.pagina() > 1) this.buscar(this.pagina() - 1);
  }

  proxima(): void {
    if (this.temMais()) this.buscar(this.pagina() + 1);
  }

  // helpers

    const estado = this.tituloService.estadoBusca;
    if (!buscou) return;
    if (!estado.buscou) return;
  }

    this.query.set(estado.query);
    this.tipoFiltro.set(estado.tipoFiltro);
    this.resultados.set(estado.resultados);
    this.pagina.set(estado.pagina);
    this.buscou.set(estado.buscou);
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
