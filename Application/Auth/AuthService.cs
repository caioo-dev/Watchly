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

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
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
    }
}