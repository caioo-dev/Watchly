using Watchly.Domain.Enum;

namespace Watchly.Application.UsuarioTitulo
{
    public interface IUsuarioTitulo
    {
        Task AddToListAsync(AddToListRequest request, CancellationToken ct);
        Task<UserTituloResponse> UpdateAsync(Guid tituloId, UpdateTrackingRequest request, CancellationToken ct);
        Task<IReadOnlyList<UserTituloResponse>> GetMyListAsync(StatusTitulo? status, CancellationToken ct);
        Task RemoveAsync(Guid tituloId, CancellationToken ct);
    }
}
