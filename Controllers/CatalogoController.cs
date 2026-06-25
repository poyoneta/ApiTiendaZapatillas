using ApiTiendaZapas.Data;
using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTiendaZapas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ZapatillasContext _context;

        public CatalogoController(ZapatillasContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODO EL CATÁLOGO -> Ruta: api/Catalogo
        [HttpGet]
        public async Task<IActionResult> ObtenerCatalogo()
        {
            var zapatillas = await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.Imagenes)
                .Include(z => z.Variantes)
                    .ThenInclude(v => v.Imagenes)
                .ToListAsync();

            return Ok(zapatillas);
        }
        
        [HttpGet("{id}")] // Esto define que la ruta es api/Catalogo/3
        public async Task<IActionResult> ObtenerProducto(int id)
        {
            var zapatilla = await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.Imagenes)
                .Include(z => z.Variantes)
                .ThenInclude(v => v.Color)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zapatilla == null) return NotFound();

            return Ok(zapatilla);
        }

        // 2. OBTENER DETALLE POR ID -> Ruta: api/Catalogo/detalle/{id}
        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var zapatilla = await _context.Zapatillas
                .Include(z => z.Marca)
                .Include(z => z.Imagenes)
                .Include(z => z.Variantes)
                    .ThenInclude(v => v.Imagenes)
                .Include(z => z.Variantes)
                    .ThenInclude(v => v.Color)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zapatilla == null)
                return NotFound();

            return Ok(zapatilla);
        }

        // 3. OBTENER FILTRADO POR MARCA -> Ruta: api/Catalogo/marca/{marcaId}
        [HttpGet("marca/{marcaId}")]
        public async Task<IActionResult> ObtenerPorMarca(int marcaId)
        {
            var zapatillas = await _context.Zapatillas
                .Include(z => z.Imagenes)
                .Include(z => z.Variantes)
                    .ThenInclude(v => v.Imagenes)
                .Where(z => z.MarcaId == marcaId)
                .ToListAsync();

            return Ok(zapatillas);
        }

        // 4. OBTENER VARIANTES -> Ruta: api/Catalogo/variantes/{zapatillaId}
        [HttpGet("variantes/{zapatillaId}")]
        public async Task<IActionResult> ObtenerVariantes(int zapatillaId)
        {
            var variantes = await _context.Variantes
                .Include(v => v.Color)
                .Include(v => v.Imagenes)
                .Where(v => v.ZapatillaId == zapatillaId)
                .ToListAsync();

            return Ok(variantes);
        }

        // 5. OBTENER TODAS LAS IMÁGENES -> Ruta: api/Catalogo/imagenes
        [HttpGet("imagenes")]
        public async Task<IActionResult> ObtenerTodasLasImagenes()
        {
            var imagenes = await _context.Imagenes.ToListAsync();
            return Ok(imagenes);
        }
        [HttpGet("marcas")]
        public async Task<IActionResult> ObtenerMarcas()
        {
            var marcas = await _context.Marcas.ToListAsync();
            return Ok(marcas);
        }
    }
}