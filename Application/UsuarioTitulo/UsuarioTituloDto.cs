using Watchly.Domain.Enum;

namespace Watchly.Application.UsuarioTitulo
{
    public sealed record AddToListRequest(Guid TituloId, StatusTitulo Status);
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
        DateTime UpdatedAt
    );
}
