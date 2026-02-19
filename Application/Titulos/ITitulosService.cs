using Watchly.Domain.Enum;

namespace Watchly.Application.Titulos
{
    public interface ITitulosService
    {
        Task<IReadOnlyList<TituloExternoResponse>> BuscarAsync(string query, TipoTitulo? tipo, CancellationToken ct);
    }
}