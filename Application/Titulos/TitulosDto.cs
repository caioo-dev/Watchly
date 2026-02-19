using Watchly.Domain.Enum;

namespace Watchly.Application.Titulos
{
    public sealed record CreateTituloRequest(TipoTitulo Tipo, string Nome, int? Ano);
    public sealed record TituloResponse(Guid Id, TipoTitulo Tipo, string Nome, int? Ano);
}
