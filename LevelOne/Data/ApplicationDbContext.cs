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
                    new PermissaoModel("Admin"),
                    new PermissaoModel("Tecnico"),
                    new PermissaoModel("Cliente")
                );
        }
    }
}
// 