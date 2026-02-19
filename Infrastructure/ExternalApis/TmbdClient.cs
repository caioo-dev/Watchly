using System.Text.Json;
using Watchly.Application.Titulos;
using Watchly.Domain.Enum;

namespace Watchly.Infrastructure.ExternalApis
{
    public sealed class TmdbClient
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private const string ImageUrl = "https://image.tmdb.org/t/p/w500";

        public TmdbClient(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<TituloExternoResponse>> SearchAsync(string query, TipoTitulo? tipo, CancellationToken ct)
        {
            var resultados = new List<TituloExternoResponse>();

            // Se tipo não for especificado ou for Filme/Serie, busca em ambos
            if (tipo is null || tipo == TipoTitulo.Filme)
                resultados.AddRange(await BuscarPorTipoAsync(query, "movie", TipoTitulo.Filme, ct));

            if (tipo is null || tipo == TipoTitulo.Serie)
                resultados.AddRange(await BuscarPorTipoAsync(query, "tv", TipoTitulo.Serie, ct));

            return resultados;
        }

        private async Task<IReadOnlyList<TituloExternoResponse>> BuscarPorTipoAsync(
            string query, string endpoint, TipoTitulo tipo, CancellationToken ct)
        {
            var url = $"{BaseUrl}/search/{endpoint}?query={Uri.EscapeDataString(query)}&language=pt-BR";
            var response = await _http.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode) return [];

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var results = doc.RootElement.GetProperty("results");

            return results.EnumerateArray()
                .Take(10)
                .Select(item =>
                {
                    var id = item.GetProperty("id").GetInt32().ToString();
                    var nome = item.TryGetProperty("title", out var t) ? t.GetString()
                                 : item.TryGetProperty("name", out var n) ? n.GetString()
                                 : null;
                    var data = item.TryGetProperty("release_date", out var rd) ? rd.GetString()
                                 : item.TryGetProperty("first_air_date", out var fd) ? fd.GetString()
                                 : null;
                    var poster = item.TryGetProperty("poster_path", out var pp) && pp.GetString() is { } path
                                 ? $"{ImageUrl}{path}" : null;
                    var ano = data is { Length: >= 4 } && int.TryParse(data[..4], out var a) ? a : (int?)null;

                    return new TituloExternoResponse(id, FonteTitulo.TMDB, tipo, nome ?? "Sem título", ano, poster);
                })
                .ToList();
        }
    }
}