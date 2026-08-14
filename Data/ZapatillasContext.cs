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
        public DbSet<ZapatillaColor> Zapatilla_Colores { get; set; }
        public DbSet<Variante> Variantes { get; set; }
        public DbSet<Imagen> Imagenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo explicito a nombres de tablas en PascalCase exactos
            modelBuilder.Entity<Marca>().ToTable("Marcas");
            modelBuilder.Entity<Zapatilla>().ToTable("Zapatillas");
            modelBuilder.Entity<Color>().ToTable("Colores");
            modelBuilder.Entity<ZapatillaColor>().ToTable("Zapatilla_Colores");
            modelBuilder.Entity<Imagen>().ToTable("Imagenes");
            modelBuilder.Entity<Variante>().ToTable("Variantes");

            // Relaciones ZapatillaColor -> Imagenes / Variantes (Borrado en cascada)
            modelBuilder.Entity<Imagen>()
                .HasOne(i => i.ZapatillaColor)
                .WithMany(zc => zc.Imagenes)
                .HasForeignKey(i => i.ZapatillaColorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Variante>()
                .HasOne(v => v.ZapatillaColor)
                .WithMany(zc => zc.Variantes)
                .HasForeignKey(v => v.ZapatillaColorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}