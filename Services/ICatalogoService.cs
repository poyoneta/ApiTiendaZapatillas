using ApiTiendaZapas.Models;

namespace ApiTiendaZapas.Services
{
    public interface ICatalogoService
    {
        Task<List<Zapatilla>> ObtenerCatalogoAsync();
        Task<Zapatilla?> ObtenerProductoAsync(int id);
        Task<List<Zapatilla>> ObtenerPorMarcaAsync(int marcaId);
        Task<List<Marca>> ObtenerMarcasAsync();
        Task<List<Imagen>> ObtenerTodasLasImagenesAsync();
        Task<List<Variante>> ObtenerVariantesPorZapatillaAsync(int zapatillaId);
    }
}