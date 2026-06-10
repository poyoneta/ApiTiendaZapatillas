using ApiTiendaZapas.Data;
using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiTiendaZapas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ZapatillasContext _context;

        public AdminController(ZapatillasContext context)
        {
            _context = context;
        }

        [HttpPost("marcas")]
        public async Task<IActionResult> CrearMarca(Marca marca)
        {
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            return Ok(marca);
        }

        [HttpPost("colores")]
        public async Task<IActionResult> CrearColor(Color color)
        {
            _context.Colores.Add(color);
            await _context.SaveChangesAsync();

            return Ok(color);
        }

        [HttpPost("zapatillas")]
        public async Task<IActionResult> CrearZapatilla(Zapatilla zapatilla)
        {
            _context.Zapatillas.Add(zapatilla);
            await _context.SaveChangesAsync();

            return Ok(zapatilla);
        }

        [HttpPost("variantes")]
        public async Task<IActionResult> CrearVariante(Variante variante)
        {
            _context.Variantes.Add(variante);
            await _context.SaveChangesAsync();

            return Ok(variante);
        }

        [HttpDelete("zapatillas/{id}")]
        public async Task<IActionResult> EliminarZapatilla(int id)
        {
            var zapatilla = await _context.Zapatillas.FindAsync(id);

            if (zapatilla == null)
                return NotFound();

            _context.Zapatillas.Remove(zapatilla);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
