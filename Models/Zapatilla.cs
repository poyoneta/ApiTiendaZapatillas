namespace ApiTiendaZapas.Models
{
    public class Zapatilla
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public int MarcaId { get; set; }

        public Marca? Marca { get; set; }
        public ICollection<Imagen> Imagenes { get; set; } = new List<Imagen>();
        public ICollection<Variante> Variantes { get; set; } = new List<Variante>();
    }
}
