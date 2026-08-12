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
                .Include(z => z.Imagenes)
                .ToListAsync();
        }

        public async Task<Zapatilla?> ObtenerPorIdAsync(int id)
        {
            return await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.Imagenes)
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == id);
        }

        public async Task<List<Zapatilla>> ObtenerPorMarcaAsync(int marcaId)
        {
            return await _context.Zapatillas
                .Include(z => z.Imagenes)
                .Include(z => z.Variantes)
                    .ThenInclude(v => v.Imagenes)
                .Where(z => z.MarcaId == marcaId)
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