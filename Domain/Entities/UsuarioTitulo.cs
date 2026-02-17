using Watchly.Domain.Enum;

namespace Watchly.Domain.Entities
{
    public class UsuarioTitulo
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid TituloId { get; private set; }
        public StatusTitulo Status { get; private set; } = StatusTitulo.ParaAssistir;
        public DateTime DataAdicionado { get; private set; }
        public int Rating { get; private set; }
        public DateTime AtualizadoEm { get; private set; } = DateTime.UtcNow;
        public int TemporadaAtual { get; private set; }
        public int EpisodioAtual { get; private set; }

        public UsuarioTitulo()
        {
            
        }

        public UsuarioTitulo(Guid id, Guid usuarioId, Guid tituloId)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id não pode ser vazio.", nameof(id));
            if (usuarioId == Guid.Empty) throw new ArgumentException("UsuarioId não pode ser vazio.", nameof(usuarioId));
            if (tituloId == Guid.Empty) throw new ArgumentException("TituloId não pode ser vazio.", nameof(tituloId));

            Id = id;
            UsuarioId = usuarioId;
            TituloId = tituloId;
        }

        public void AvaliarTitulo(int rating)
        {
            if (rating < 1 || rating > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Avaliação deve ser entre 1 e 10.");
            }

            Rating = rating;
            AtualizadoEmData();
        }

        public void AtualizarStatus(StatusTitulo novoStatus)
        {
            Status = novoStatus;
            AtualizadoEmData();
        }

        public void AtualizarProgresso(int temporada, int episodio)
        {
            TemporadaAtual = temporada;
            EpisodioAtual = episodio;

            if (Status == StatusTitulo.ParaAssistir) Status = StatusTitulo.Assistindo;
            AtualizadoEmData();
        }

        public void AtualizadoEmData()
        {
            AtualizadoEm = DateTime.UtcNow;
        }
    }
}
