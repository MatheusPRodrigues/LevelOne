using LevelOne.Models;
using Microsoft.EntityFrameworkCore;

namespace LevelOne.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<PermissaoModel> Permissoes { get; set; }
        public DbSet<UsuarioPermissaoModel> UsuariosPermissoes { get; set; }
        public DbSet<ChamadoModel> Chamados { get; set; }
        public DbSet<MensagemModel> Mensagens { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioPermissaoModel>().
                HasKey(up => new
                {
                    up.UsuarioId,
                    up.PermissaoId
                });

            modelBuilder.Entity<UsuarioPermissaoModel>()
                .HasOne(up => up.Usuario)
                .WithMany(u => u.UsuarioPermissoes)
                .HasForeignKey(up => up.UsuarioId);

            modelBuilder.Entity<UsuarioPermissaoModel>()
                .HasOne(up => up.Permissao)
                .WithMany(p => p.UsuarioPermissoes)
                .HasForeignKey(up => up.PermissaoId);

            modelBuilder.Entity<PermissaoModel>()
                .HasData(
                    new PermissaoModel(1, "Admin"),
                    new PermissaoModel(2, "Tecnico"),
                    new PermissaoModel(3, "Cliente")
                );

            modelBuilder.Entity<ChamadoModel>()
                .HasMany(c => c.Mensagens)
                .WithOne(m => m.Chamado)
                .HasForeignKey(m => m.ChamadoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioModel>()
                .HasMany<MensagemModel>()
                .WithOne(m => m.Usuario)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
