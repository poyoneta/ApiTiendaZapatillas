using ApiTiendaZapas.Models;
using ApiTiendaZapas.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaZapas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("marcas")]
        public async Task<IActionResult> CrearMarca(Marca marca)
        {
            var creada = await _adminService.CrearMarcaAsync(marca);
            return Ok(creada);
        }

        [HttpPost("colores")]
        public async Task<IActionResult> CrearColor(Color color)
        {
            var creado = await _adminService.CrearColorAsync(color);
            return Ok(creado);
        }

        [HttpPost("zapatillas")]
        public async Task<IActionResult> CrearZapatilla(Zapatilla zapatilla)
        {
            var creada = await _adminService.CrearZapatillaAsync(zapatilla);
            return Ok(creada);
        }

        [HttpPost("variantes")]
        public async Task<IActionResult> CrearVariante(Variante variante)
        {
            var creada = await _adminService.CrearVarianteAsync(variante);
            return Ok(creada);
        }

        [HttpPost("subir-imagen")]
        public async Task<IActionResult> CrearImagen([FromForm] FormSubirImagen modelo)
        {
            if (modelo.Archivo == null || modelo.Archivo.Length == 0)
                return BadRequest("No se proporcionó ningún archivo de imagen.");

            if (modelo.Id_zapatilla == null && modelo.Id_variante == null)
                return BadRequest("La imagen debe estar asociada a una Zapatilla o a una Variante.");

            try
            {
                var imagen = await _adminService.SubirImagenAsync(
                    modelo.Archivo, modelo.Orden, modelo.Id_zapatilla, modelo.Id_variante);

                return Ok(imagen);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar la imagen: {ex.Message}");
            }
        }

        [HttpDelete("zapatillas/{id}")]
        public async Task<IActionResult> EliminarZapatilla(int id)
        {
            var eliminada = await _adminService.EliminarZapatillaAsync(id);

            if (!eliminada)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("variantes/{id}")]
        public async Task<IActionResult> EliminarVariante(int id)
        {
            var eliminada = await _adminService.EliminarVarianteAsync(id);

            if (!eliminada)
                return NotFound();

            return NoContent();
        }
    }

    public class FormSubirImagen
    {
        public IFormFile? Archivo { get; set; }
        public int Orden { get; set; }
        public int? Id_zapatilla { get; set; }
        public int? Id_variante { get; set; }
    }
}