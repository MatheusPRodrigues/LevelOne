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
    }
}
// 