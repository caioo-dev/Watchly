
namespace Watchly.Application.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
        Task<MeResponse> GetMeAsync(CancellationToken ct);
        Task<MeResponse> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct);
    }
}