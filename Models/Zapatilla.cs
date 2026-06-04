namespace ApiTiendaZapas.Models
{
    public class Zapatilla
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }

        public int MarcaId { get; set; }

        public Marca? Marca { get; set; }
    }
}
