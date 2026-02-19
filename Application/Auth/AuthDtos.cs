
namespace Watchly.Application.Auth
{
    public sealed record RegisterRequest(string Email, string Senha);
    public sealed record LoginRequest(string Email, string Senha);
    public sealed record AuthResponse(string Token);
}