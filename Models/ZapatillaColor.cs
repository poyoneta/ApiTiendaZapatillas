namespace ApiTiendaZapas.Models
{
    public class ZapatillaColor
    {
        public int Id { get; set; }

        public int ZapatillaId { get; set; }
        public Zapatilla? Zapatilla { get; set; }

        public int ColorId { get; set; }
        public Color? Color { get; set; }

        public ICollection<Imagen> Imagenes { get; set; } = new List<Imagen>();
        public ICollection<Variante> Variantes { get; set; } = new List<Variante>();
    }
}