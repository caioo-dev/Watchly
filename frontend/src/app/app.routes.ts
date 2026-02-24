import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'busca',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then(m => m.RegisterComponent)
  },
 
  {
    path: 'busca',
    loadComponent: () => import('./features/busca/busca').then(m => m.BuscaComponent)
  },
  {
    path: 'minha-lista',
    loadComponent: () => import('./features/minha-lista/minha-lista').then(m => m.MinhaListaComponent),
    canActivate: [authGuard]
  },
       /*
  {
    path: 'perfil',
    loadComponent: () => import('../features/perfil/perfil').then(m => m.PerfilComponent),
    canActivate: [authGuard]
  },
    */
  {
    path: 'titulo/:fonte/:externalId',
    loadComponent: () => import('./features/titulo-detalhe/titulo-detalhe').then(m => m.TituloDetalheComponent)
  },

  {
    path: '**',
    redirectTo: 'busca'
  }
];