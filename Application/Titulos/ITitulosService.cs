using Watchly.Domain.Enum;

namespace Watchly.Application.Titulos
{
    public interface ITitulosService
    {
        Task<TituloResponse> CriarAsync(CreateTituloRequest request, CancellationToken ct);
        Task<IReadOnlyList<TituloResponse>> BuscarAsync(string? query, TipoTitulo? tipo, CancellationToken ct);
    }
}
