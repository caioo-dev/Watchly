using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Watchly.Domain.Entities;
using Watchly.Infrastructure;

namespace Watchly.Application.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _httpContextAccessor = new HttpContextAccessor(
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            var emailNormalizado = request.Email.Trim().ToLowerInvariant();

            var jaExiste = await _db.Usuarios.AnyAsync(u => u.Email == emailNormalizado, ct);
            if (jaExiste)
                throw new InvalidOperationException("E-mail já cadastrado.");

            var hash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            var usuario = new Usuario(emailNormalizado, hash);

            await _db.Usuarios.AddAsync(usuario, ct);
            await _db.SaveChangesAsync(ct);

            return new AuthResponse(GerarToken(usuario));
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var emailNormalizado = request.Email.Trim().ToLowerInvariant();

            var usuario = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Email == emailNormalizado, ct);

            if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

            return new AuthResponse(GerarToken(usuario));
        }

        private string GerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private Guid GetUsuarioId()
        {
            var claim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(claim) || !Guid.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return userId;
        }

        public async Task<MeResponse> GetMeAsync(CancellationToken ct)
        {
            var usuarioId = GetUsuarioId();

            var usuario = await _db.Usuarios.FindAsync([usuarioId], ct)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            return new MeResponse(usuario.Id, usuario.Email, usuario.CriadoEm);
        }

        public async Task<MeResponse> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct)
        {
            var usuarioId = GetUsuarioId();

            var usuario = await _db.Usuarios.FindAsync([usuarioId], ct)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailNormalizado = request.Email.Trim().ToLowerInvariant();
                var emailEmUso = await _db.Usuarios
                    .AnyAsync(u => u.Email == emailNormalizado && u.Id != usuarioId, ct);

                if (emailEmUso)
                    throw new InvalidOperationException("E-mail já está em uso.");

                usuario.AtualizarEmail(emailNormalizado);
            }

            if (!string.IsNullOrWhiteSpace(request.NovaSenha))
            {
                if (string.IsNullOrWhiteSpace(request.SenhaAtual))
                    throw new ArgumentException("Senha atual é obrigatória para alterar a senha.");

                if (!BCrypt.Net.BCrypt.Verify(request.SenhaAtual, usuario.SenhaHash))
                    throw new UnauthorizedAccessException("Senha atual incorreta.");

                usuario.AtualizarSenha(BCrypt.Net.BCrypt.HashPassword(request.NovaSenha));
            }

            await _db.SaveChangesAsync(ct);
            return new MeResponse(usuario.Id, usuario.Email, usuario.CriadoEm);
        }
    }
}