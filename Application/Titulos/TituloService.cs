using Watchly.Domain.Enum;
using Watchly.Infrastructure.ExternalApis;

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
    }
}