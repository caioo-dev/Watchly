using Microsoft.EntityFrameworkCore;
using Watchly.Domain.Entities;

namespace Watchly.Infrastructure
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Titulo> Titulos => Set<Titulo>();
        public DbSet<UsuarioTitulo> UsuarioTitulos => Set<UsuarioTitulo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Titulo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.Tipo).HasConversion<string>().IsRequired();
                entity.Property(e => e.Ano);
            });

            modelBuilder.Entity<UsuarioTitulo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UsuarioId).IsRequired();
                entity.Property(e => e.TituloId).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasConversion<string>();
                entity.Property(e => e.DataAdicionado).IsRequired();
                entity.Property(e => e.Nota);
                entity.Property(e => e.AtualizadoEm).IsRequired();
                entity.Property(e => e.TemporadaAtual);
                entity.Property(e => e.EpisodioAtual);

                entity.HasOne(ut => ut.Titulo)
                    .WithMany()
                    .HasForeignKey(ut => ut.TituloId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
