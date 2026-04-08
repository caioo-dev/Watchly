export type TipoTitulo = 'Filme' | 'Serie' | 'Anime';
export type FonteTitulo = 'TMDB';
export type StatusTitulo = 'ParaAssistir' | 'Assistindo' | 'Concluido' | 'Abandonado';

export interface TituloExternoResponse {
  externalId: string;
  fonte: FonteTitulo;
  tipo: TipoTitulo;
  nome: string;
  ano?: number;
  imagemUrl?: string;
  poularidade?: number;
}

export interface TituloDetalheResponse {
  externalId: string;
  fonte: FonteTitulo;
  tipo: TipoTitulo;
  nome: string;
  ano?: number;
  imagemUrl?: string;
  sinopse?: string;
  popularidade?: number;
}