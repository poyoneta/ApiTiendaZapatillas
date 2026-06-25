using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiTiendaZapas.Models
{
    public class Imagen
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int? Orden { get; set; }

        // Relación con Zapatilla
        public int? Id_zapatilla { get; set; }
        public Zapatilla? Zapatilla { get; set; }

        // Relación con Variante (Cambiado a singular)
        public int? Id_variante { get; set; }
        public Variante? Variante { get; set; }
    }
}