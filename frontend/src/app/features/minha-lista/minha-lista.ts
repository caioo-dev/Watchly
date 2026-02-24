import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MinhaListaService } from '../../core/services/minha-lista.service';
import { AuthService } from '../../core/services/auth.service';
import { UserTituloResponse } from '../../core/models/usuario-titulo.model';
import { StatusTitulo } from '../../core/models/titulo.model';

@Component({
  selector: 'app-minha-lista',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './minha-lista.html'
})
export class MinhaListaComponent implements OnInit {
  private readonly minhaListaService = inject(MinhaListaService);
  private readonly cdr = inject(ChangeDetectorRef);
  readonly authService = inject(AuthService);

  lista: UserTituloResponse[] = [];
  carregando = true;
  erro = '';
  filtroStatus: StatusTitulo | '' = '';

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;

    this.minhaListaService.buscarLista(this.filtroStatus || undefined).subscribe({
      next: (data) => {
        this.lista = data;
        this.carregando = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.erro = 'Erro ao carregar lista.';
        this.carregando = false;
        this.cdr.detectChanges();
      }
    });
  }

  remover(tituloId: string): void {
    this.minhaListaService.remover(tituloId).subscribe({
      next: () => {
        this.lista = this.lista.filter(t => t.tituloId !== tituloId);
        this.cdr.detectChanges();
      }
    });
  }

  atualizarStatus(tituloId: string, status: StatusTitulo): void {
    this.minhaListaService.atualizar(tituloId, { status }).subscribe({
      next: (atualizado) => {
        const index = this.lista.findIndex(t => t.tituloId === tituloId);
        if (index !== -1) {
          this.lista[index] = atualizado;
          this.cdr.detectChanges();
        }
      }
    });
  }
}