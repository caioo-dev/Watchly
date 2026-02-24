import { FonteTitulo, StatusTitulo, TipoTitulo } from './titulo.model';

export interface AddToListRequest {
  externalId: string;
  fonte: FonteTitulo;
  tipo: TipoTitulo;
  nome: string;
  ano?: number;
  imagemUrl?: string;
  status: StatusTitulo;
}

export interface UpdateTrackingRequest {
  status?: StatusTitulo;
  nota?: number;
  notas?: string;
  temporadaAtual?: number;
  episodioAtual?: number;
}

export interface UserTituloResponse {
  tituloId: string;
  nome: string;
  tipo: TipoTitulo;
  status: StatusTitulo;
  nota?: number;
  temporadaAtual?: number;
  episodioAtual?: number;
  imagemUrl?: string;
  updatedAt: string;
}