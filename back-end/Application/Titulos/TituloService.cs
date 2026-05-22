using Watchly.Domain.Enum;
using Watchly.Infrastructure.ExternalApis;

namespace Watchly.Application.Titulos
{
    public sealed class TituloService : ITitulosService
    {
        private readonly TmdbClient _tmdb;

        public TituloService(TmdbClient tmdb)
        {
            _tmdb = tmdb;
        }

        public async Task<IReadOnlyList<TituloExternoResponse>> BuscarAsync(
            string busca, TipoTitulo? tipo, int page, CancellationToken ct)
        {
            return await _tmdb.SearchAsync(busca, tipo, page, ct);
        }

        public async Task<TituloDetalheResponse> BuscarDetalheAsync(
            FonteTitulo fonte, TipoTitulo tipo, string externalId, CancellationToken ct)
        {
            TituloDetalheResponse? detalhe = await _tmdb.GetDetalheAsync(externalId, tipo, ct);
            return detalhe ?? throw new KeyNotFoundException("Título não encontrado na fonte externa.");
        }
    }
}