using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Http;

namespace ApiTiendaZapas.Services
{
    public interface IAdminService
    {
        Task<Marca> CrearMarcaAsync(Marca marca);
        Task<Color> CrearColorAsync(Color color);
        Task<Zapatilla> CrearZapatillaAsync(Zapatilla zapatilla);
        Task<Variante> CrearVarianteAsync(Variante variante);
        Task<Imagen> SubirImagenAsync(IFormFile archivo, int orden, int? idZapatilla, int? idVariante);

        // Devuelve false si la zapatilla no existe
        Task<bool> EliminarZapatillaAsync(int id);

        // Devuelve false si la variante no existe
        Task<bool> EliminarVarianteAsync(int id);
    }
}