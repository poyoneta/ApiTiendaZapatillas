using ApiTiendaZapas.Models;

namespace ApiTiendaZapas.Repositories
{
    public interface IZapatillaRepository
    {
        // 1. Catálogo liviano para el Index
        Task<List<Zapatilla>> ObtenerTodasSimplificadasAsync();

        // 2. Detalle del producto (con colores e imágenes)
        Task<Zapatilla?> ObtenerPorIdConColoresAsync(int id);

        // 3. Variantes/Talles de un color específico
        Task<List<Variante>> ObtenerVariantesPorColorwayAsync(int zapatillaColorId);

        // Consultas auxiliares
        Task<List<Zapatilla>> ObtenerPorMarcaAsync(int marcaId);
        Task<List<Marca>> ObtenerMarcasAsync();
        Task<List<Imagen>> ObtenerTodasLasImagenesAsync();
    }
}