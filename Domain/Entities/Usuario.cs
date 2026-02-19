namespace Watchly.Domain.Entities
{
    public sealed class Usuario
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Email { get; private set; } = string.Empty;
        public string SenhaHash { get; private set; } = string.Empty;
        public DateTime CriadoEm { get; private set; } = DateTime.UtcNow;

        public Usuario() { }

        public Usuario(string email, string senhaHash)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email não pode ser vazio.", nameof(email));

            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("Senha não pode ser vazia.", nameof(senhaHash));

            Email = email.Trim().ToLowerInvariant();
            SenhaHash = senhaHash;
        }
        public void AtualizarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email não pode ser vazio.", nameof(email));
            Email = email;
        }

        public void AtualizarSenha(string senhaHash)
        {
            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("Senha não pode ser vazia.", nameof(senhaHash));
            SenhaHash = senhaHash;
        }
    }
}