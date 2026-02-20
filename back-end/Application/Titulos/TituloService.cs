using System.Text.Json;
using Watchly.Domain.Enum;
using Watchly.Infrastructure.ExternalApis;
using static System.Net.WebRequestMethods;

namespace Watchly.Application.Titulos
{
    public sealed class TituloService : ITitulosService
    {
        private readonly TmdbClient _tmdb;
        private readonly JikanClient _jikan;

        public TituloService(TmdbClient tmdb, JikanClient jikan)
        {
            _tmdb = tmdb;
            _jikan = jikan;
        }

        public async Task<IReadOnlyList<TituloExternoResponse>> BuscarAsync(
            string query, TipoTitulo? tipo, CancellationToken ct)
        {
            var tasks = new List<Task<IReadOnlyList<TituloExternoResponse>>>();

            // TMDB cuida de Filme e Serie
            if (tipo is null || tipo == TipoTitulo.Filme || tipo == TipoTitulo.Serie)
                tasks.Add(_tmdb.SearchAsync(query, tipo, ct));

            // Jikan cuida de Anime
            if (tipo is null || tipo == TipoTitulo.Anime)
                tasks.Add(_jikan.SearchAsync(query, ct));

            var resultados = await Task.WhenAll(tasks);

            return resultados.SelectMany(r => r).ToList().AsReadOnly();
        }

        public async Task<TituloDetalheResponse> BuscarDetalheAsync(
            string externalId, FonteTitulo fonte, CancellationToken ct)
        {
            TituloDetalheResponse? detalhe = fonte switch
            {
                FonteTitulo.TMDB => await _tmdb.GetDetalheAsync(externalId, TipoTitulo.Filme, ct)
                                  ?? await _tmdb.GetDetalheAsync(externalId, TipoTitulo.Serie, ct),
                FonteTitulo.Jikan => await _jikan.GetDetalheAsync(externalId, ct),
                _ => throw new ArgumentException("Fonte inválida.")
            };

            return detalhe ?? throw new KeyNotFoundException("Título não encontrado na fonte externa.");
        }
    }
}