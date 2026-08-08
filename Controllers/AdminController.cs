using ApiTiendaZapas.Data;
using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTiendaZapas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ZapatillasContext _context;
        private readonly IConfiguration _configuration;

        // Inyectamos IConfiguration para leer Supabase:Url y Supabase:AnonKey
        public AdminController(ZapatillasContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                // 2. Obtener la configuración de Supabase desde appsettings.Development.json
                string supabaseUrl = _configuration["Supabase:Url"]!;
                string anonKey = _configuration["Supabase:AnonKey"]!;
                string bucketName = "zapatillas-imagenes"; // Nombre exacto del bucket público en Supabase

                // 3. Generar un nombre único para la imagen
                string nombreUnico = $"{Guid.NewGuid()}{Path.GetExtension(modelo.Archivo.FileName)}";

                // 4. Subir la imagen a Supabase Storage mediante petición HTTP
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {anonKey}");
                client.DefaultRequestHeaders.Add("apiKey", anonKey);

                using var stream = modelo.Archivo.OpenReadStream();
                using var content = new StreamContent(stream);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(modelo.Archivo.ContentType);

                // AGREGAMOS ?upsert=true AL FINAL DE LA RUTA
                string uploadEndpoint = $"{supabaseUrl}/storage/v1/object/{bucketName}/{nombreUnico}?upsert=true";
                var response = await client.PostAsync(uploadEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error al subir a Supabase Storage: {errorResponse}");
                }

                // 5. Construir la URL pública de la imagen alojada en Supabase
                string urlPublica = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{nombreUnico}";

                // 6. Guardar la referencia con la URL pública en PostgreSQL
                var nuevaImagen = new Imagen
                {
                    Url = urlPublica,
                    Orden = modelo.Orden,
                    Id_zapatilla = (modelo.Id_zapatilla == 0) ? null : modelo.Id_zapatilla,
                    Id_variante = (modelo.Id_variante == 0) ? null : modelo.Id_variante
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

    public class FormSubirImagen
    {
        public IFormFile? Archivo { get; set; }
        public int Orden { get; set; }
        public int? Id_zapatilla { get; set; }
        public int? Id_variante { get; set; }
    }
}