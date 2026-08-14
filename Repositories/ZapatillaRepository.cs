using ApiTiendaZapas.Data;
using ApiTiendaZapas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiTiendaZapas.Repositories
{
    public class ZapatillaRepository : IZapatillaRepository
    {
        private readonly ZapatillasContext _context;

        public ZapatillaRepository(ZapatillasContext context)
        {
            _context = context;
        }

        // 1. Obtiene las zapatillas con SOLO la imagen principal (para las tarjetas del index)
        public async Task<List<Zapatilla>> ObtenerTodasSimplificadasAsync()
        {
            return await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Imagenes.Where(i => i.Es_Principal))
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        // 2. Obtiene la zapatilla seleccionada con sus colores y todas las fotos de cada color
        public async Task<Zapatilla?> ObtenerPorIdConColoresAsync(int id)
        {
            return await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Color)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Imagenes)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(z => z.Id == id);
        }

        // 3. Obtiene únicamente los talles, precio y stock de un colorway al hacer clic en el color
        public async Task<List<Variante>> ObtenerVariantesPorColorwayAsync(int zapatillaColorId)
        {
            return await _context.Variantes
                .Where(v => v.ZapatillaColorId == zapatillaColorId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Zapatilla>> ObtenerPorMarcaAsync(int marcaId)
        {
            return await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Imagenes.Where(i => i.Es_Principal))
                .Where(z => z.MarcaId == marcaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Marca>> ObtenerMarcasAsync()
        {
            return await _context.Marcas.AsNoTracking().ToListAsync();
        }

        public async Task<List<Imagen>> ObtenerTodasLasImagenesAsync()
        {
            return await _context.Imagenes.AsNoTracking().ToListAsync();
        }
    }
}