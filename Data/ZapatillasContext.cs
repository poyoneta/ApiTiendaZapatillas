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
    }
}
