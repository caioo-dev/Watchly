using Microsoft.EntityFrameworkCore;
using Watchly.Domain.Entities;

namespace Watchly.Infrastructure
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Titulos> Titulos => Set<Titulos>();
        public DbSet<UsuarioTitulo> UsuarioTitulos => Set<UsuarioTitulo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Titulos>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.Tipo).IsRequired();
                entity.Property(e => e.Ano);
            });

            modelBuilder.Entity<UsuarioTitulo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UsuarioId).IsRequired();
                entity.Property(e => e.TituloId).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.DataAdicionado).IsRequired();
                entity.Property(e => e.Rating);
                entity.Property(e => e.AtualizadoEm).IsRequired();
                entity.Property(e => e.TemporadaAtual);
                entity.Property(e => e.EpisodioAtual);

                entity.HasOne<Titulos>()
                    .WithMany()
                    .HasForeignKey(ut => ut.TituloId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
