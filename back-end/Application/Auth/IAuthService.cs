
namespace Watchly.Application.Auth
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request, CancellationToken ct);
        Task LoginAsync(LoginRequest request, CancellationToken ct);
        void Logout();
        Task<MeResponse> GetMeAsync(CancellationToken ct);
        Task<MeResponse> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct);
    }
}