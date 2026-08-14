namespace ApiTiendaZapas.Models
{
    public class Variante
    {
        public int Id { get; set; }

        public int ZapatillaColorId { get; set; }
        public ZapatillaColor? ZapatillaColor { get; set; }

        public int Talla { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
