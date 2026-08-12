using ApiTiendaZapas.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaZapas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogoController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCatalogo()
        {
            var zapatillas = await _catalogoService.ObtenerCatalogoAsync();
            return Ok(zapatillas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerProducto(int id)
        {
            var zapatilla = await _catalogoService.ObtenerProductoAsync(id);
            if (zapatilla == null) return NotFound();
            return Ok(zapatilla);
        }

        [HttpGet("marca/{marcaId}")]
        public async Task<IActionResult> ObtenerPorMarca(int marcaId)
        {
            var zapatillas = await _catalogoService.ObtenerPorMarcaAsync(marcaId);
            return Ok(zapatillas);
        }

        [HttpGet("imagenes")]
        public async Task<IActionResult> ObtenerTodasLasImagenes()
        {
            var imagenes = await _catalogoService.ObtenerTodasLasImagenesAsync();
            return Ok(imagenes);
        }

        [HttpGet("marcas")]
        public async Task<IActionResult> ObtenerMarcas()
        {
            var marcas = await _catalogoService.ObtenerMarcasAsync();
            return Ok(marcas);
        }
    }
}