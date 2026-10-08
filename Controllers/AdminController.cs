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

        [HttpPost("colorways")]
        public async Task<IActionResult> CrearZapatillaColor(ZapatillaColor zapatillaColor)
        {
            var creado = await _adminService.CrearZapatillaColorAsync(zapatillaColor);
            return Ok(creado);
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

            if (modelo.ZapatillaColorId <= 0)
                return BadRequest("La imagen debe estar asociada a un ZapatillaColorId válido.");

            try
            {
                var imagen = await _adminService.SubirImagenAsync(
                    modelo.Archivo, modelo.Orden, modelo.Es_Principal, modelo.ZapatillaColorId);

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

        [HttpPut("zapatillas/{id}")]
        public async Task<IActionResult> EditarZapatilla(int id, [FromBody] ActualizarZapatillaDto dto)
        {
            try
            {
                var editada = await _adminService.EditarZapatillaAsync(id, dto.MarcaId, dto.Nombre, dto.Descripcion);
                if (!editada)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("variantes/{id}")]
        public async Task<IActionResult> EditarVariante(int id, [FromBody] ActualizarVarianteDto dto)
        {
            try
            {
                var editada = await _adminService.EditarVarianteAsync(id, dto.Talla, dto.Precio, dto.Stock);
                if (!editada)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("imagenes/{id}/principal")]
        public async Task<IActionResult> MarcarImagenPrincipal(int id)
        {
            var ok = await _adminService.MarcarImagenPrincipalAsync(id);
            if (!ok)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("colorways/{id}")]
        public async Task<IActionResult> EliminarZapatillaColor(int id)
        {
            var eliminado = await _adminService.EliminarZapatillaColorAsync(id);
            if (!eliminado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("imagenes/{id}")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var eliminada = await _adminService.EliminarImagenAsync(id);
            if (!eliminada)
                return NotFound();

            return NoContent();
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

    public class ActualizarZapatillaDto
    {
        public int? MarcaId { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ActualizarVarianteDto
    {
        public int? Talla { get; set; }
        public decimal? Precio { get; set; }
        public int? Stock { get; set; }
    }

    public class FormSubirImagen
    {
        public IFormFile? Archivo { get; set; }
        public int Orden { get; set; }
        public bool Es_Principal { get; set; }
        public int ZapatillaColorId { get; set; }
    }
}