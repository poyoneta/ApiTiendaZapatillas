using ApiTiendaZapas.Data;
using Microsoft.AspNetCore.Http;
using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ApiTiendaZapas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly ZapatillasContext _context;

        public InventarioController(ZapatillasContext context)
        {
            _context = context;
        }

        [HttpGet("zapatilla/{id}")]
        public async Task<IActionResult> ObtenerVariantes(int id)
        {
            var variantes = await _context.Variantes
                .Include(v => v.Color)
                .Where(v => v.ZapatillaId == id)
                .ToListAsync();

            return Ok(variantes);
        }

        [HttpGet("stock-bajo")]
        public async Task<IActionResult> StockBajo()
        {
            var variantes = await _context.Variantes
                .Include(v => v.Color)
                .Where(v => v.Stock < 5)
                .ToListAsync();

            return Ok(variantes);
        }

        [HttpPut("stock/{id}")]
        public async Task<IActionResult> ActualizarStock(int id, int nuevoStock)
        {
            var variante = await _context.Variantes.FindAsync(id);

            if (variante == null)
                return NotFound();

            variante.Stock = nuevoStock;

            await _context.SaveChangesAsync();

            return Ok(variante);
        }
    }
}
