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

        public async Task<List<Zapatilla>> ObtenerTodasAsync()
        {
            return await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Color)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Imagenes)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Variantes)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Zapatilla?> ObtenerPorIdAsync(int id)
        {
            return await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Color)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Imagenes)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Variantes)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == id);
        }

        public async Task<List<Zapatilla>> ObtenerPorMarcaAsync(int marcaId)
        {
            return await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Color)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Imagenes)
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Variantes)
                .Where(z => z.MarcaId == marcaId)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<Variante>> ObtenerVariantesPorZapatillaAsync(int zapatillaId)
        {
            return await _context.Variantes
                .Include(v => v.ZapatillaColor)
                    .ThenInclude(zc => zc!.Color)
                .Where(v => v.ZapatillaColor!.ZapatillaId == zapatillaId)
                .ToListAsync();
        }

        public async Task<List<Marca>> ObtenerMarcasAsync()
        {
            return await _context.Marcas.ToListAsync();
        }

        public async Task<List<Imagen>> ObtenerTodasLasImagenesAsync()
        {
            return await _context.Imagenes.ToListAsync();
        }
    }
}