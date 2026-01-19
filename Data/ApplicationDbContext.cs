using Microsoft.EntityFrameworkCore;
using poocrud.Models;

namespace poocrud.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Clientes> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Clientes>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombres).HasColumnName("nombres").HasColumnType("nvarchar(100)");
                entity.Property(e => e.Cedula).HasColumnName("cedula").HasColumnType("nvarchar(10)");
                entity.Property(e => e.Direccion).HasColumnName("direccion").HasColumnType("varchar(200)");
                entity.Property(e => e.Email).HasColumnName("email").HasColumnType("varchar(200)");
                entity.Property(e => e.Telf).HasColumnName("telf").HasColumnType("varchar(10)");
            });
        }
    }
}
