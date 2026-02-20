
namespace Watchly.Application.Auth
{
    public sealed record RegisterRequest(string Email, string Senha);
    public sealed record LoginRequest(string Email, string Senha);
    public sealed record AuthResponse(string Token);
    public sealed record MeResponse(Guid Id, string Email, DateTime CriadoEm);
    public sealed record UpdateProfileRequest(string? Email, string? SenhaAtual, string? NovaSenha);
}