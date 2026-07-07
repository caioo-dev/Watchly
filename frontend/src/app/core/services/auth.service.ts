import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';
import { AuthResponse, LoginRequest, MeResponse, RegisterRequest, UpdateProfileRequest } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly _usuario = signal<MeResponse | null>(null);
  readonly usuario = this._usuario.asReadonly();

  register(request: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/auth/register`, request);
  }

  login(request: LoginRequest): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/auth/login`, request);
  }

  getMe(): Observable<MeResponse> {
    return this.http.get<MeResponse>(`${environment.apiUrl}/auth/me`).pipe(
      tap(response => this._usuario.set(response))
    );
  }

  updateProfile(request: UpdateProfileRequest): Observable<MeResponse> {
    return this.http.put<MeResponse>(`${environment.apiUrl}/auth/me`, request).pipe(
      tap(response => this._usuario.set(response))
    );
  }

  logout(): void {
    this.http.post<void>(`${environment.apiUrl}/auth/logout`, {}).subscribe({
      next: () => this.finalizarLogout(),
      error: () => this.finalizarLogout() // cookie pode já ter expirado; desloga localmente de qualquer forma
    });
  }

  isAuthenticated(): boolean {
    return this._usuario() !== null;
  }

  private finalizarLogout(): void {
    this._usuario.set(null);
    this.router.navigate(['/login']);
  }
}