import { Component, inject, ChangeDetectorRef } from '@angular/core';
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
export class BuscaComponent {
  private readonly tituloService = inject(TituloService);
  private readonly cdr = inject(ChangeDetectorRef);

  query = '';
  tipoFiltro: TipoTitulo | '' = '';
  resultados: TituloExternoResponse[] = [];
  carregando = false;
  erro = '';
  buscou = false;

  buscar(): void {
    if (!this.query.trim()) return;

    this.carregando = true;
    this.erro = '';
    this.buscou = true;

    this.tituloService.buscar(this.query, this.tipoFiltro || undefined).subscribe({
      next: (data) => {
        this.resultados = data;
        this.carregando = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.erro = 'Erro ao buscar títulos.';
        this.carregando = false;
        this.cdr.detectChanges();
      }
    });
  }
}