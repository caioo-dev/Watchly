using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Watchly.Domain.Entities;
using Watchly.Domain.Enum;
using Watchly.Infrastructure;

namespace Watchly.Application.UsuarioTitulo
{
    public sealed class UsuarioTituloService : IUsuarioTitulo
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioTituloService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        // ──────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────

        private Guid GetUsuarioId()
        {
            var claim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(claim) || !Guid.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return userId;
        }

        private async Task<Domain.Entities.UsuarioTitulo> GetEntryOrThrowAsync(
            Guid usuarioId, Guid tituloId, CancellationToken ct)
        {
            var entry = await _db.UsuarioTitulos
                .Include(ut => ut.Titulo)
                .FirstOrDefaultAsync(ut => ut.UsuarioId == usuarioId && ut.TituloId == tituloId, ct);

            if (entry is null)
                throw new KeyNotFoundException("Título não encontrado na lista do usuário.");

            return entry;
        }

        // ──────────────────────────────────────────────
        // Interface implementation
        // ──────────────────────────────────────────────

        public async Task AddToListAsync(AddToListRequest request, CancellationToken ct)
        {
            var usuarioId = GetUsuarioId();

            // Upsert do título
            var titulo = await _db.Titulos
                .FirstOrDefaultAsync(t => t.ExternalId == request.ExternalId && t.Fonte == request.Fonte, ct);

            if (titulo is null)
            {
                titulo = new Titulo(request.ExternalId, request.Fonte, request.Tipo, request.Nome, request.Ano, request.ImagemUrl);
                await _db.Titulos.AddAsync(titulo, ct);
                await _db.SaveChangesAsync(ct);
            }

            var jaAdicionado = await _db.UsuarioTitulos.AnyAsync(
                ut => ut.UsuarioId == usuarioId && ut.TituloId == titulo.Id, ct);

            if (jaAdicionado)
                throw new InvalidOperationException("Título já está na lista do usuário.");

            var entry = new Domain.Entities.UsuarioTitulo(Guid.NewGuid(), usuarioId, titulo.Id);
            entry.AtualizarStatus(request.Status);

            await _db.UsuarioTitulos.AddAsync(entry, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<UserTituloResponse> UpdateAsync(
            Guid tituloId, UpdateTrackingRequest request, CancellationToken ct)
        {
            var usuarioId = GetUsuarioId();
            var entry = await GetEntryOrThrowAsync(usuarioId, tituloId, ct);

            if (request.Status.HasValue)
                entry.AtualizarStatus(request.Status.Value);

            if (request.Nota.HasValue)
                entry.AvaliarTitulo(request.Nota.Value);

            if (request.TemporadaAtual.HasValue || request.EpisodioAtual.HasValue)
            {
                var temporada = request.TemporadaAtual ?? entry.TemporadaAtual;
                var episodio = request.EpisodioAtual ?? entry.EpisodioAtual;
                entry.AtualizarProgresso(temporada, episodio);
            }

            await _db.SaveChangesAsync(ct);

            return MapToResponse(entry);
        }

        public async Task<IReadOnlyList<UserTituloResponse>> GetMyListAsync(
            StatusTitulo? status, CancellationToken ct)
        {
            var usuarioId = GetUsuarioId();

            var query = _db.UsuarioTitulos
                .Include(ut => ut.Titulo)
                .Where(ut => ut.UsuarioId == usuarioId);

            if (status.HasValue)
                query = query.Where(ut => ut.Status == status.Value);

            var list = await query
                .OrderByDescending(ut => ut.AtualizadoEm)
                .ToListAsync(ct);

            return list.Select(MapToResponse).ToList().AsReadOnly();
        }

        public async Task RemoveAsync(Guid tituloId, CancellationToken ct)
        {
            var usuarioId = GetUsuarioId();
            var entry = await GetEntryOrThrowAsync(usuarioId, tituloId, ct);

            _db.UsuarioTitulos.Remove(entry);
            await _db.SaveChangesAsync(ct);
        }

        // ──────────────────────────────────────────────
        // Mapper
        // ──────────────────────────────────────────────

        private static UserTituloResponse MapToResponse(Domain.Entities.UsuarioTitulo ut) =>
            new(
                ut.TituloId,
                ut.Titulo.Nome,
                ut.Titulo.Tipo,
                ut.Status,
                ut.Nota,
                ut.TemporadaAtual == 0 ? null : ut.TemporadaAtual,
                ut.EpisodioAtual == 0 ? null : ut.EpisodioAtual,
                ut.Titulo.ImagemUrl,
                ut.AtualizadoEm
            );
    }
}