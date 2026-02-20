using Watchly.Domain.Enum;

namespace Watchly.Application.Titulos
{
    public sealed record TituloExternoResponse(
        string ExternalId,
        FonteTitulo Fonte,
        TipoTitulo Tipo,
        string Nome,
        int? Ano,
        string? ImagemUrl
    );
    public sealed record TituloDetalheResponse(
        string ExternalId,
        FonteTitulo Fonte,
        TipoTitulo Tipo,
        string Nome,
        int? Ano,
        string? ImagemUrl,
        string? Sinopse,
        double? Popularidade
    );
}
