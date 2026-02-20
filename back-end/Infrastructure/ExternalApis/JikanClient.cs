using System.Text.Json;
using Watchly.Application.Titulos;
using Watchly.Domain.Enum;

namespace Watchly.Infrastructure.ExternalApis
{
    public sealed class JikanClient
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "https://api.jikan.moe/v4";

        public JikanClient(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<TituloExternoResponse>> SearchAsync(string query, CancellationToken ct)
        {
            var url = $"{BaseUrl}/anime?q={Uri.EscapeDataString(query)}&limit=10";
            var response = await _http.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode) return [];

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var data = doc.RootElement.GetProperty("data");

            return data.EnumerateArray()
                .Select(item =>
                {
                    var id = item.GetProperty("mal_id").GetInt32().ToString();
                    var nome = item.TryGetProperty("title_portuguese", out var tp) && !string.IsNullOrWhiteSpace(tp.GetString())
                                ? tp.GetString()!
                                : item.GetProperty("title").GetString() ?? "Sem título";
                    var ano = item.TryGetProperty("year", out var y) && y.ValueKind == JsonValueKind.Number
                                ? y.GetInt32() : (int?)null;
                    var imagem = item.TryGetProperty("images", out var imgs)
                               && imgs.TryGetProperty("jpg", out var jpg)
                               && jpg.TryGetProperty("image_url", out var imgUrl)
                                ? imgUrl.GetString() : null;

                    return new TituloExternoResponse(id, FonteTitulo.Jikan, TipoTitulo.Anime, nome, ano, imagem);
                })
                .ToList();
        }

        public async Task<TituloDetalheResponse?> GetDetalheAsync(string externalId, CancellationToken ct)
        {
            var url = $"{BaseUrl}/anime/{externalId}";
            var response = await _http.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            var item = doc.RootElement.GetProperty("data");

            var nome = item.TryGetProperty("title_portuguese", out var tp) && !string.IsNullOrWhiteSpace(tp.GetString())
                        ? tp.GetString()! : item.GetProperty("title").GetString() ?? "Sem título";
            var sinopse = item.TryGetProperty("synopsis", out var s) ? s.GetString() : null;
            var ano = item.TryGetProperty("year", out var y) && y.ValueKind == JsonValueKind.Number
                        ? y.GetInt32() : (int?)null;
            var pop = item.TryGetProperty("popularity", out var p) && p.ValueKind == JsonValueKind.Number
                        ? p.GetDouble() : (double?)null;
            var imagem = item.TryGetProperty("images", out var imgs)
                       && imgs.TryGetProperty("jpg", out var jpg)
                       && jpg.TryGetProperty("image_url", out var imgUrl)
                        ? imgUrl.GetString() : null;

            return new TituloDetalheResponse(externalId, FonteTitulo.Jikan, TipoTitulo.Anime, nome, ano, imagem, sinopse, pop);
        }
    }
}