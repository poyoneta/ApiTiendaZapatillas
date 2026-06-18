using ApiTiendaZapas.Data;
using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<IActionResult> ObtenerCatalogo()
        {
            var zapatillas = await _context.Zapatillas
                .Include(z => z.Marca)
                .ToListAsync();

            return Ok(zapatillas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var zapatilla = await _context.Zapatillas
                .Include(z => z.Marca)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zapatilla == null)
                return NotFound();

            return Ok(zapatilla);
        }

        [HttpGet("marca/{marcaId}")]
        public async Task<IActionResult> ObtenerPorMarca(int marcaId)
        {
            var zapatillas = await _context.Zapatillas
                .Include(z => z.Marca)
                .Where(z => z.MarcaId == marcaId)
                .ToListAsync();

            return Ok(zapatillas);
        }
        [HttpGet("{zapatillaId}/variantes")]
        public async Task<IActionResult> ObtenerVariantes(int zapatillaId)
        {
            var variantes = await _context.Variantes
                .Include(v => v.Color)
                .Where(v => v.ZapatillaId == zapatillaId)
                .ToListAsync();

            return Ok(variantes);
        }

    }
    }



