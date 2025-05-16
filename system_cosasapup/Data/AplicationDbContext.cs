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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<pegues>()
                .HasOne(p => p.pagos)
                .WithMany(p => p.pegues)
                .HasForeignKey(p => p.idPago)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
