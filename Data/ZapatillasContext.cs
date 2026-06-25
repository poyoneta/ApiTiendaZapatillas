using Microsoft.EntityFrameworkCore;
using ApiTiendaZapas.Models;

namespace ApiTiendaZapas.Data
{
    public class ZapatillasContext : DbContext
    {
        public ZapatillasContext(DbContextOptions<ZapatillasContext> options)
        : base(options)
        {
        }

        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Zapatilla> Zapatillas { get; set; }
        public DbSet<Color> Colores { get; set; }
        public DbSet<Variante> Variantes { get; set; }
        public DbSet<Imagen> Imagenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo Zapatilla -> Imagen (Mantiene el borrado en cascada)
            modelBuilder.Entity<Imagen>()
                .HasOne(i => i.Zapatilla)
                .WithMany(z => z.Imagenes)
                .HasForeignKey(i => i.Id_zapatilla)
                .OnDelete(DeleteBehavior.Cascade);

            // Mapeo Variante -> Imagen (Cambiamos a NoAction para que SQL Server no proteste)
            modelBuilder.Entity<Imagen>()
                .HasOne(i => i.Variante)
                .WithMany(v => v.Imagenes)
                .HasForeignKey(i => i.Id_variante)
                .OnDelete(DeleteBehavior.NoAction); // <-- ESTE CAMBIO DESTRABA TODO
        }
    }
}
