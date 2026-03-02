using System.Text.Json;
using Watchly.Domain.Enum;
using Watchly.Infrastructure.ExternalApis;
using static System.Net.WebRequestMethods;

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
            string query, TipoTitulo? tipo, int page, CancellationToken ct)
        {
            return await _tmdb.SearchAsync(query, tipo, page, ct);
        }

        public async Task<TituloDetalheResponse> BuscarDetalheAsync(
            FonteTitulo fonte, TipoTitulo tipo, string externalId, CancellationToken ct)
        {
            var detalhe = await _tmdb.GetDetalheAsync(externalId, tipo, ct);
            return detalhe ?? throw new KeyNotFoundException("Título não encontrado na fonte externa.");
        }
    }
}