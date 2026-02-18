using Watchly.Domain.Enum;

namespace Watchly.Domain.Entities
{
    public class Titulos
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public TipoTitulo Tipo { get; private set; }
        public string Nome { get; private set; } = default!;
        public int? Ano { get; private set; }

        public Titulos()
        {
            
        }

        public Titulos(TipoTitulo tipo, string nome, int? ano = null)
        {
            SetTipo(tipo);
            SetNome(nome);
            SetAno(ano);
        }

        public void RenomearTitulo(string novoNome) => SetNome(novoNome);

        public void AlterarAno(int? ano) => SetAno(ano);

        public void AlterarTipo(TipoTitulo tipo) => SetTipo(tipo);

        public void SetTipo(TipoTitulo tipo)
        {
            if (!System.Enum.IsDefined(typeof(TipoTitulo), tipo))
                throw new ArgumentOutOfRangeException(nameof(tipo), "Tipo inválido.");

            Tipo = tipo;
        }

        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do título não pode ser vazio.", nameof(nome));
            Nome = nome.Trim();
        }

        public void SetAno(int? ano)
        {

            var currentYear = DateTime.UtcNow.Year;

            if (ano is null)
            {
                Ano = default;
                return;
            }

            if (ano.HasValue && (ano < 1888 || ano > currentYear + 5))
            {
                throw new ArgumentOutOfRangeException(nameof(ano), $"O ano deve ser entre 1888 e {currentYear + 1}.");
            }
            Ano = ano ?? default;
        }
    }
}
