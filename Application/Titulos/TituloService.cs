using Watchly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Watchly.Domain.Enum;
using Watchly.Infrastructure;

namespace Watchly.Application.Titulos
{
    public sealed class TituloService : ITitulosService
    {
        private readonly AppDbContext _db;

        public TituloService(AppDbContext db) => _db = db;

        public async Task<TituloResponse> CriarAsync(CreateTituloRequest request, CancellationToken ct)
        {
            var titulo = new Titulo(request.Tipo, request.Nome, request.Ano);

            _db.Titulos.Add(titulo);
            await _db.SaveChangesAsync(ct);

            return new TituloResponse(titulo.Id, titulo.Tipo, titulo.Nome, titulo.Ano);
        }

        public async Task<IReadOnlyList<TituloResponse>> BuscarAsync(string? query, TipoTitulo? tipo, CancellationToken ct)
        {
            var q = _db.Titulos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var term = query.Trim();
                q = q.Where(x => x.Nome.Contains(term));
            }

            if (tipo is not null)
                q = q.Where(x => x.Tipo == tipo);

            return await q
                .OrderBy(x => x.Nome)
                .Select(x => new TituloResponse(x.Id, x.Tipo, x.Nome, x.Ano))
                .ToListAsync(ct);
        }
    }
}
