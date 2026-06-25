using ApiTiendaZapas.Data;
using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApiTiendaZapas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ZapatillasContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ZapatillasContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

        // Cambiamos "imagenes" por "subir-imagen" para destrabar a Swagger definitivamente
        [HttpPost("subir-imagen")]
        public async Task<IActionResult> CrearImagen([FromForm] FormSubirImagen modelo)
        {
            // 1. Validaciones básicas de entrada
            if (modelo.Archivo == null || modelo.Archivo.Length == 0)
                return BadRequest("No se proporcionó ningún archivo de imagen.");

            if (modelo.Id_zapatilla == null && modelo.Id_variante == null)
                return BadRequest("La imagen debe estar asociada a una Zapatilla o a una Variante.");

            try
            {
                // 2. Definir la ruta en wwwroot/uploads
                string baseRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string folderPath = Path.Combine(baseRoot, "uploads");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 3. Generar un nombre único para que no se pisen los archivos
                string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(modelo.Archivo.FileName);
                string filePath = Path.Combine(folderPath, nombreUnico);

                // 4. Guardar físicamente el archivo en el disco
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await modelo.Archivo.CopyToAsync(stream);
                }

                // 5. Guardar la referencia en la Base de Datos
                string urlRelativa = $"/uploads/{nombreUnico}";

                var nuevaImagen = new Imagen
                {
                    Url = urlRelativa,
                    Orden = modelo.Orden,
                    Id_zapatilla = modelo.Id_zapatilla ?? 0, // Si es null, usa 0
                    Id_variante = (modelo.Id_variante == 0) ? null : modelo.Id_variante // ¡ESTO ES CLAVE!
                };

                _context.Imagenes.Add(nuevaImagen);
                await _context.SaveChangesAsync();

                return Ok(nuevaImagen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar la imagen: {ex.Message}");
            }
        }

        // PEGÁ ESTA CLASE AUXILIAR JUSTO ABAJO DEL MÉTODO ANTERIOR (O AL FINAL DEL CONTROLADOR)
        public class FormSubirImagen
        {
            // Al agregar el ? le decimos a .NET que puede arrancar vacía y se va el cartel amarillo
            public IFormFile? Archivo { get; set; }
            public int Orden { get; set; }
            public int? Id_zapatilla { get; set; }
            public int? Id_variante { get; set; }
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
