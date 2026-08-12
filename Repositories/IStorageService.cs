using Microsoft.AspNetCore.Http;

namespace ApiTiendaZapas.Services
{
    public interface IStorageService
    {
        Task<string> SubirArchivoAsync(IFormFile archivo);
        Task BorrarArchivoAsync(string urlPublica);
    }
}