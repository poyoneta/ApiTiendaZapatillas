using ApiTiendaZapas.Models;
using ApiTiendaZapas.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaZapas.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly IZapatillaRepository _zapatillaRepo;

        public CatalogoService(IZapatillaRepository zapatillaRepo)
        {
            _zapatillaRepo = zapatillaRepo;
        }

        public async Task<List<Zapatilla>> ObtenerCatalogoAsync()
        {
            return await _zapatillaRepo.ObtenerTodasAsync();
        }

        public async Task<Zapatilla?> ObtenerProductoAsync(int id)
        {
            return await _zapatillaRepo.ObtenerPorIdAsync(id);
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
        public async Task<List<Variante>> ObtenerVariantesPorZapatillaAsync(int zapatillaId)
        {
            return await _zapatillaRepo.ObtenerVariantesPorZapatillaAsync(zapatillaId);
        }
    }
}
