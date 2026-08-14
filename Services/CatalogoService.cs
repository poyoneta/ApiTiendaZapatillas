using ApiTiendaZapas.Models;
using ApiTiendaZapas.Repositories;

namespace ApiTiendaZapas.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly IZapatillaRepository _zapatillaRepo;

        public CatalogoService(IZapatillaRepository zapatillaRepo)
        {
            _zapatillaRepo = zapatillaRepo;
        }

        public async Task<List<Zapatilla>> ObtenerCatalogoSimplificadoAsync()
        {
            return await _zapatillaRepo.ObtenerTodasSimplificadasAsync();
        }

        public async Task<Zapatilla?> ObtenerPorIdConColoresAsync(int id)
        {
            return await _zapatillaRepo.ObtenerPorIdConColoresAsync(id);
        }

        public async Task<List<Variante>> ObtenerVariantesPorColorwayAsync(int zapatillaColorId)
        {
            return await _zapatillaRepo.ObtenerVariantesPorColorwayAsync(zapatillaColorId);
        }

        public async Task<List<Zapatilla>> ObtenerPorMarcaAsync(int marcaId)
        {
            return await _zapatillaRepo.ObtenerPorMarcaAsync(marcaId);
        }

        public async Task<List<Marca>> ObtenerMarcasAsync()
        {
            return await _zapatillaRepo.ObtenerMarcasAsync();
        }

        public async Task<List<Imagen>> ObtenerTodasLasImagenesAsync()
        {
            return await _zapatillaRepo.ObtenerTodasLasImagenesAsync();
        }
    }
}