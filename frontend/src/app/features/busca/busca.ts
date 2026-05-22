import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router, NavigationStart } from '@angular/router';
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
  private readonly cdr           = inject(ChangeDetectorRef);

  busca      = '';
  tipoFiltro: TipoTitulo | '' = '';
  resultados: TituloExternoResponse[] = [];
  carregando = false;
  erro       = '';
  buscou     = false;
  pagina     = 1;
  temMais    = true;

  ngOnInit(): void {
    if (this.tituloService.voltandoDaBusca) {
      this.tituloService.voltandoDaBusca = false;
      this.restaurarEstado();
    } else {
      this.tituloService.limparEstado();
    }
  }

  limpar(): void {
    this.busca      = '';
    this.tipoFiltro = '';
    this.resultados = [];
    this.buscou     = false;
    this.pagina     = 1;
    this.temMais    = true;
    this.erro       = '';
    this.tituloService.limparEstado();
    this.cdr.detectChanges();
  }

  buscar(pagina = 1): void {
    if (!this.busca.trim()) return;

    this.carregando = true;
    this.erro       = '';
    this.buscou     = true;
    this.pagina     = pagina;

    this.tituloService.buscar(this.busca, this.tipoFiltro || undefined, pagina).subscribe({
      next: (data) => this.onBuscaSuccess(data),
      error: ()     => this.onBuscaError()
    });
  }

  anterior(): void {
    if (this.pagina > 1) this.buscar(this.pagina - 1);
  }

  proxima(): void {
    if (this.temMais) this.buscar(this.pagina + 1);
  }

  // ──────────────────────────────────────────────
  // Helpers
  // ──────────────────────────────────────────────

  private restaurarEstado(): void {
    const { busca, tipoFiltro, resultados, pagina, buscou } = this.tituloService.estadoBusca;

    if (!buscou) return;

    this.busca      = busca;
    this.tipoFiltro = tipoFiltro;
    this.resultados = resultados;
    this.pagina     = pagina;
    this.buscou     = buscou;
    this.cdr.detectChanges();
  }

  private onBuscaSuccess(data: TituloExternoResponse[]): void {
    this.resultados = data;
    this.temMais    = data.length > 0;
    this.carregando = false;
    this.cdr.detectChanges();
  }

  private onBuscaError(): void {
    this.erro       = 'Erro ao buscar títulos.';
    this.carregando = false;
    this.cdr.detectChanges();
  }
}