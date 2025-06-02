using Microsoft.EntityFrameworkCore;
using system_cosasapup.Models;

namespace system_cosasapup.Data
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<pegues> pegues { get; set; }
        public DbSet<pagos> pagos { get; set; }
        public DbSet<usuarios> usuarios { get; set; }


        //Login aqui modelo
        public DbSet<Usuario> usuario { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definir la clave primaria de la entidad pegues
            modelBuilder.Entity<pegues>()
                .HasKey(p => p.PegueId);  // Marca PegueId como la clave primaria

            // Configurar la relación con pagos
            modelBuilder.Entity<pegues>()
                .HasMany(p => p.pagos)
                .WithOne(p => p.Pegue)
                .HasForeignKey(p => p.PegueId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
