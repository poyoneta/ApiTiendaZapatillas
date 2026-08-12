using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApiTiendaZapas.Services
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StorageService> _logger;
        private readonly string _bucketName = "zapatillas-imagenes";

        public StorageService(IConfiguration configuration, ILogger<StorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private HttpClient CrearClienteAutenticado()
        {
            string anonKey = _configuration["Supabase:AnonKey"]!;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {anonKey}");
            client.DefaultRequestHeaders.Add("apiKey", anonKey);
            return client;
        }

        public async Task<string> SubirArchivoAsync(IFormFile archivo)
        {
            string supabaseUrl = _configuration["Supabase:Url"]!;
            string nombreUnico = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";

            using var client = CrearClienteAutenticado();
            using var stream = archivo.OpenReadStream();
            using var content = new StreamContent(stream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(archivo.ContentType);

            string uploadEndpoint = $"{supabaseUrl}/storage/v1/object/{_bucketName}/{nombreUnico}?upsert=true";
            var response = await client.PostAsync(uploadEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Error al subir a Supabase Storage: {errorResponse}");
            }

            return $"{supabaseUrl}/storage/v1/object/public/{_bucketName}/{nombreUnico}";
        }

        public async Task BorrarArchivoAsync(string urlPublica)
        {
            if (string.IsNullOrWhiteSpace(urlPublica))
            {
                _logger.LogWarning("Se intentó borrar una imagen sin URL — se omite para no arriesgar el bucket.");
                return;
            }

            string supabaseUrl = _configuration["Supabase:Url"]!;
            string prefijoEsperado = $"{supabaseUrl}/storage/v1/object/public/{_bucketName}/";

            // Guard crítico: si la URL no tiene el formato esperado de este bucket,
            // NO intentamos borrar nada — evita que un path vacío o mal armado
            // termine pegándole a un endpoint que borre de más.
            if (!urlPublica.StartsWith(prefijoEsperado))
            {
                _logger.LogWarning("URL con formato inesperado, no se borra por seguridad: {Url}", urlPublica);
                return;
            }

            string nombreArchivo = urlPublica.Substring(prefijoEsperado.Length);

            if (string.IsNullOrWhiteSpace(nombreArchivo))
            {
                _logger.LogWarning("No se pudo extraer un nombre de archivo válido de: {Url}", urlPublica);
                return;
            }

            using var client = CrearClienteAutenticado();
            string deleteEndpoint = $"{supabaseUrl}/storage/v1/object/{_bucketName}/{nombreArchivo}";

            try
            {
                var response = await client.DeleteAsync(deleteEndpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("No se pudo borrar {Archivo} del bucket: {Error}", nombreArchivo, errorResponse);
                }
                else
                {
                    _logger.LogInformation("Archivo borrado del bucket: {Archivo}", nombreArchivo);
                }
            }
            catch (Exception ex)
            {
                // No bloqueamos el borrado de la zapatilla si falla el borrado de un archivo puntual
                _logger.LogWarning(ex, "Excepción al borrar {Archivo} del bucket", nombreArchivo);
            }
        }
    }
}