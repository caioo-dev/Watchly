import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    senha: ['', [Validators.required, Validators.minLength(6)]]
  });

  erro = '';
  carregando = false;

  onSubmit(): void {
    if (this.form.invalid) return;

    this.carregando = true;
    this.erro = '';

    this.authService.login(this.form.value as any).subscribe({
      next: () => this.router.navigate(['/busca']),
      error: (err) => {
        this.erro = err.error?.message ?? 'Erro ao fazer login.';
        this.carregando = false;
      }
    });
  }
}