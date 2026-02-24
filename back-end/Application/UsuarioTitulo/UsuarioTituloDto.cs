using Watchly.Domain.Enum;

namespace Watchly.Application.UsuarioTitulo
{
    public sealed record AddToListRequest(
        string ExternalId,
        FonteTitulo Fonte,
        TipoTitulo Tipo,
        string Nome,
        int? Ano,
        string? ImagemUrl,
        StatusTitulo Status
    );
    public sealed record UpdateTrackingRequest(
        StatusTitulo? Status,
        int? Nota,
        string? Notas,
        int? TemporadaAtual,
        int? EpisodioAtual
    );

    public sealed record UserTituloResponse(
        Guid TituloId,
        string Nome,
        TipoTitulo Tipo,
        StatusTitulo Status,
        int? Nota,
        int? TemporadaAtual,
        int? EpisodioAtual,
        string? ImagemUrl,
        DateTime UpdatedAt
    );
}
