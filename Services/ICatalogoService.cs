using ApiTiendaZapas.Models;

namespace ApiTiendaZapas.Services
{
    public interface ICatalogoService
    {
        Task<List<Zapatilla>> ObtenerCatalogoSimplificadoAsync();
        Task<Zapatilla?> ObtenerPorIdConColoresAsync(int id);
        Task<List<Variante>> ObtenerVariantesPorColorwayAsync(int zapatillaColorId);
        Task<List<Zapatilla>> ObtenerPorMarcaAsync(int marcaId);
        Task<List<Marca>> ObtenerMarcasAsync();
        Task<List<Imagen>> ObtenerTodasLasImagenesAsync();
    }
}