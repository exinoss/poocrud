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
                entity.Property(e => e.TipoDocumento).HasColumnName("tipo_documento").HasColumnType("nvarchar(20)");
                entity.Property(e => e.Documento).HasColumnName("documento").HasColumnType("nvarchar(13)");
                entity.Property(e => e.Direccion).HasColumnName("direccion").HasColumnType("varchar(200)");
                entity.Property(e => e.Ciudad).HasColumnName("ciudad").HasColumnType("nvarchar(100)");
                entity.Property(e => e.Email).HasColumnName("email").HasColumnType("varchar(200)");
                entity.Property(e => e.Telf).HasColumnName("telf").HasColumnType("varchar(15)");
                entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento").HasColumnType("date");
                entity.Property(e => e.Estado).HasColumnName("estado").HasColumnType("nvarchar(20)").HasDefaultValue("Activo");
                entity.Property(e => e.CreadoEn).HasColumnName("creado_en").HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
