using Watchly.Domain.Enum;

namespace Watchly.Domain.Entities
{
    public class UsuarioTitulo
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UsuarioId { get; private set; }
        public Guid TituloId { get; private set; }
        public StatusTitulo Status { get; private set; } = StatusTitulo.ParaAssistir;
        public DateTime DataAdicionado { get; private set; }
        public int Nota { get; private set; }
        public DateTime AtualizadoEm { get; private set; } = DateTime.UtcNow;
        public int TemporadaAtual { get; private set; }
        public int EpisodioAtual { get; private set; }
        public Titulo Titulo { get; private set; } = default!;


        public UsuarioTitulo()
        {
            
        }

        public UsuarioTitulo(Guid id, Guid usuarioId, Guid tituloId)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("UsuarioId não pode ser vazio.", nameof(usuarioId));
            if (tituloId == Guid.Empty) throw new ArgumentException("TituloId não pode ser vazio.", nameof(tituloId));

            UsuarioId = usuarioId;
            TituloId = tituloId;
            TituloId = tituloId;
            DataAdicionado = DateTime.UtcNow;
            AtualizadoEm = DateTime.UtcNow;
        }

        public void AvaliarTitulo(int rating)
        {
            if (rating < 1 || rating > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Avaliação deve ser entre 1 e 10.");
            }

            Nota = rating;
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
