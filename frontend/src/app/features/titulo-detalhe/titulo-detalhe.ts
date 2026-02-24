import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TituloService } from '../../core/services/titulo.service';
import { MinhaListaService } from '../../core/services/minha-lista.service';
import { AuthService } from '../../core/services/auth.service';
import { TituloDetalheResponse, FonteTitulo, StatusTitulo } from '../../core/models/titulo.model';

@Component({
  selector: 'app-titulo-detalhe',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './titulo-detalhe.html'
})
export class TituloDetalheComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly tituloService = inject(TituloService);
  private readonly minhaListaService = inject(MinhaListaService);
  private readonly cdr = inject(ChangeDetectorRef);
  readonly authService = inject(AuthService);

  titulo: TituloDetalheResponse | null = null;
  carregando = true;
  erro = '';
  adicionando = false;
  adicionado = false;
  mensagem = '';

  ngOnInit(): void {
    const fonte = this.route.snapshot.paramMap.get('fonte') as FonteTitulo;
    const externalId = this.route.snapshot.paramMap.get('externalId')!;

    this.tituloService.buscarDetalhe(fonte, externalId).subscribe({
      next: (data) => {
        this.titulo = data;
        this.carregando = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.erro = 'Erro ao carregar título.';
        this.carregando = false;
        this.cdr.detectChanges();
      }
    });
  }

  adicionarNaLista(status: StatusTitulo): void {
    if (!this.titulo) return;

    this.adicionando = true;

    this.minhaListaService.adicionar({
      externalId: this.titulo.externalId,
      fonte: this.titulo.fonte,
      tipo: this.titulo.tipo,
      nome: this.titulo.nome,
      ano: this.titulo.ano,
      imagemUrl: this.titulo.imagemUrl,
      status
    }).subscribe({
      next: () => {
        this.adicionado = true;
        this.mensagem = 'Adicionado à sua lista!';
        this.adicionando = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.mensagem = err.error?.message ?? 'Erro ao adicionar.';
        this.adicionando = false;
        this.cdr.detectChanges();
      }
    });
  }
}