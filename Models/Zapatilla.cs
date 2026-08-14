namespace ApiTiendaZapas.Models
{
    public class Zapatilla
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public int MarcaId { get; set; }
        public Marca? Marca { get; set; }

        // Relación con la tabla puente (Colorways)
        public ICollection<ZapatillaColor> ZapatillaColores { get; set; } = new List<ZapatillaColor>();
    }
}
