using Watchly.Domain.Enum;

namespace Watchly.Domain.Entities
{
    public sealed class Titulo
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string ExternalId { get; private set; } = string.Empty;
        public FonteTitulo Fonte { get; private set; }
        public TipoTitulo Tipo { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public int? Ano { get; private set; }
        public string? ImagemUrl { get; private set; }

        public Titulo() { }

        public Titulo(string externalId, FonteTitulo fonte, TipoTitulo tipo, string nome, int? ano, string? imagemUrl)
        {
            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("ExternalId não pode ser vazio.", nameof(externalId));
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome não pode ser vazio.", nameof(nome));

            ExternalId = externalId;
            Fonte = fonte;
            Tipo = tipo;
            Nome = nome;
            Ano = ano;
            ImagemUrl = imagemUrl;
        }
    }
}