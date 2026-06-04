namespace ApiTiendaZapas.Models
{
    public class Variante
    {
        public int Id { get; set; }

        public int ZapatillaId { get; set; }
        public int ColorId { get; set; }

        public int Talla { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public Zapatilla? Zapatilla { get; set; }
        public Color? Color { get; set; }
    }
}
