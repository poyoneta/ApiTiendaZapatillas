using System.ComponentModel.DataAnnotations;

namespace ApiTiendaZapas.Models
{
    public class Imagen
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int? Orden { get; set; }
        public bool Es_Principal { get; set; }

        // Relación con ZapatillaColor
        public int ZapatillaColorId { get; set; }
        public ZapatillaColor? ZapatillaColor { get; set; }
    }
}