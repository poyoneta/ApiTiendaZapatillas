using ApiTiendaZapas.Models;

namespace ApiTiendaZapas.Repositories
{
    public interface IZapatillaRepository
    {
        Task<List<Zapatilla>> ObtenerTodasAsync();
        Task<Zapatilla?> ObtenerPorIdAsync(int id);
        Task<List<Zapatilla>> ObtenerPorMarcaAsync(int marcaId);
        Task<List<Marca>> ObtenerMarcasAsync();
        Task<List<Imagen>> ObtenerTodasLasImagenesAsync();
        Task<List<Variante>> ObtenerVariantesPorZapatillaAsync(int zapatillaId);
    }
}