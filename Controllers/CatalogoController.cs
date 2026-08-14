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

        // 1. GET: api/catalogo (Para el Index -> Trae tarjetas ultralivianas)
        [HttpGet]
        public async Task<IActionResult> ObtenerCatalogo()
        {
            var zapatillas = await _catalogoService.ObtenerCatalogoSimplificadoAsync();
            return Ok(zapatillas);
        }

        // 2. GET: api/catalogo/5 (Click en zapatilla -> Trae colores y fotos)
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerProducto(int id)
        {
            var zapatilla = await _catalogoService.ObtenerPorIdConColoresAsync(id);
            if (zapatilla == null) return NotFound();
            return Ok(zapatilla);
        }

        // 3. GET: api/catalogo/colorway/12/variantes (Click en un color -> Trae talles y stock)
        [HttpGet("colorway/{zapatillaColorId}/variantes")]
        public async Task<IActionResult> ObtenerVariantesPorColor(int zapatillaColorId)
        {
            var variantes = await _catalogoService.ObtenerVariantesPorColorwayAsync(zapatillaColorId);
            return Ok(variantes);
        }

        // Endpoints auxiliares
        [HttpGet("marca/{marcaId}")]
        public async Task<IActionResult> ObtenerPorMarca(int marcaId)
        {
            var zapatillas = await _catalogoService.ObtenerPorMarcaAsync(marcaId);
            return Ok(zapatillas);
        }

        [HttpGet("marcas")]
        public async Task<IActionResult> ObtenerMarcas()
        {
            var marcas = await _catalogoService.ObtenerMarcasAsync();
            return Ok(marcas);
        }

        [HttpGet("imagenes")]
        public async Task<IActionResult> ObtenerTodasLasImagenes()
        {
            var imagenes = await _catalogoService.ObtenerTodasLasImagenesAsync();
            return Ok(imagenes);
        }
    }
}