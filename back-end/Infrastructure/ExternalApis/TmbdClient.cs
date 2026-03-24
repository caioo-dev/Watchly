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
        private const int GeneroAnimacao = 16;
        private const string OrigemJapao = "JP";

        public TmdbClient(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<TituloExternoResponse>> SearchAsync(
            string query, TipoTitulo? tipo, int page, CancellationToken ct)
        {
            var resultados = await BuscarResultadosAsync(query, tipo, page, ct);
            return FiltrarPorTipo(resultados, tipo);
        }

        public async Task<TituloDetalheResponse?> GetDetalheAsync(
            string externalId, TipoTitulo tipo, CancellationToken ct)
        {
            var endpoint = ResolverEndpointDetalhe(tipo);
            var url = $"{BaseUrl}/{endpoint}/{externalId}?language=pt-BR";
            var response = await _http.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            return ParsearDetalhe(doc.RootElement, externalId, tipo);
        }

        private async Task<List<TituloExternoResponse>> BuscarResultadosAsync(
            string query, TipoTitulo? tipo, int page, CancellationToken ct)
        {
            var resultados = new List<TituloExternoResponse>();

            if (DeveABuscarFilmes(tipo))
                resultados.AddRange(await BuscarPorEndpointAsync(query, "movie", TipoTitulo.Filme, page, ct));

            if (DeveBuscarSeries(tipo))
                resultados.AddRange(await BuscarPorEndpointAsync(query, "tv", TipoTitulo.Serie, page, ct));

            return resultados;
        }

        private async Task<IReadOnlyList<TituloExternoResponse>> BuscarPorEndpointAsync(
            string query, string endpoint, TipoTitulo tipoBase, int page, CancellationToken ct)
        {
            var url = $"{BaseUrl}/search/{endpoint}?query={Uri.EscapeDataString(query)}&language=pt-BR&page={page}";
            var response = await _http.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode) return [];

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            return doc.RootElement
                .GetProperty("results")
                .EnumerateArray()
                .Select(item => ParsearItem(item, tipoBase))
                .ToList();
        }


        private static IReadOnlyList<TituloExternoResponse> FiltrarPorTipo(
            List<TituloExternoResponse> resultados, TipoTitulo? tipo) => tipo switch
            {
                TipoTitulo.Anime => resultados.Where(r => r.Tipo == TipoTitulo.Anime).ToList().AsReadOnly(),
                TipoTitulo.Serie => resultados.Where(r => r.Tipo == TipoTitulo.Serie).ToList().AsReadOnly(),
                _ => resultados.AsReadOnly()
            };

        private static bool DeveABuscarFilmes(TipoTitulo? tipo) =>
            tipo is null || tipo == TipoTitulo.Filme;

        private static bool DeveBuscarSeries(TipoTitulo? tipo) =>
            tipo is null || tipo == TipoTitulo.Serie || tipo == TipoTitulo.Anime;



        private static TituloExternoResponse ParsearItem(JsonElement item, TipoTitulo tipoBase)
        {
            var id = item.GetProperty("id").GetInt32().ToString();
            var nome = ExtrairNome(item);
            var ano = ExtrairAno(item);
            var poster = ExtrairPoster(item);
            var tipo = ClassificarTipo(item, tipoBase);

            return new TituloExternoResponse(id, FonteTitulo.TMDB, tipo, nome, ano, poster);
        }

        private static TituloDetalheResponse ParsearDetalhe(
            JsonElement item, string externalId, TipoTitulo tipo)
        {
            var nome = ExtrairNome(item);
            var ano = ExtrairAno(item);
            var poster = ExtrairPoster(item);
            var sinopse = item.TryGetProperty("overview", out var o) ? o.GetString() : null;
            var popul = item.TryGetProperty("popularity", out var p) ? p.GetDouble() : (double?)null;

            return new TituloDetalheResponse(externalId, FonteTitulo.TMDB, tipo, nome, ano, poster, sinopse, popul);
        }



        private static TipoTitulo ClassificarTipo(JsonElement item, TipoTitulo tipoBase)
        {
            if (tipoBase == TipoTitulo.Filme) return TipoTitulo.Filme;

            var generos = ExtrairGeneros(item);
            var origens = ExtrairOrigens(item);
            var eAnime = generos.Contains(GeneroAnimacao) && origens.Contains(OrigemJapao);

            return eAnime ? TipoTitulo.Anime : TipoTitulo.Serie;
        }

        private static string ExtrairNome(JsonElement item)
        {
            if (item.TryGetProperty("title", out var title) && !string.IsNullOrWhiteSpace(title.GetString()))
                return title.GetString()!;

            if (item.TryGetProperty("name", out var name) && !string.IsNullOrWhiteSpace(name.GetString()))
                return name.GetString()!;

            return "Sem título";
        }

        private static int? ExtrairAno(JsonElement item)
        {
            var data = item.TryGetProperty("release_date", out var rd) ? rd.GetString()
                     : item.TryGetProperty("first_air_date", out var fd) ? fd.GetString()
                     : null;

            return data is { Length: >= 4 } && int.TryParse(data[..4], out var ano) ? ano : null;
        }

        private static string? ExtrairPoster(JsonElement item) =>
            item.TryGetProperty("poster_path", out var pp) && pp.GetString() is { } path
                ? $"{ImageUrl}{path}"
                : null;

        private static List<int> ExtrairGeneros(JsonElement item) =>
            item.TryGetProperty("genre_ids", out var g)
                ? g.EnumerateArray().Select(x => x.GetInt32()).ToList()
                : [];

        private static List<string?> ExtrairOrigens(JsonElement item) =>
            item.TryGetProperty("origin_country", out var oc)
                ? oc.EnumerateArray().Select(x => x.GetString()).ToList()
                : [];

        private static string ResolverEndpointDetalhe(TipoTitulo tipo) =>
            tipo == TipoTitulo.Filme ? "movie" : "tv";
    }
}