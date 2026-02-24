export interface RegisterRequest {
  email: string;
  senha: string;
}

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface AuthResponse {
  token: string;
}

export interface MeResponse {
  id: string;
  email: string;
  criadoEm: string;
}

export interface UpdateProfileRequest {
  email?: string;
  senhaAtual?: string;
  novaSenha?: string;
}