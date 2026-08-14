using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Http;

namespace ApiTiendaZapas.Services
{
    public interface IAdminService
    {
        Task<Marca> CrearMarcaAsync(Marca marca);
        Task<Color> CrearColorAsync(Color color);
        Task<Zapatilla> CrearZapatillaAsync(Zapatilla zapatilla);
        Task<ZapatillaColor> CrearZapatillaColorAsync(ZapatillaColor zapatillaColor);
        Task<Variante> CrearVarianteAsync(Variante variante);
        Task<Imagen> SubirImagenAsync(IFormFile archivo, int orden, bool esPrincipal, int zapatillaColorId);

        Task<bool> EliminarZapatillaAsync(int id);
        Task<bool> EliminarVarianteAsync(int id);
    }
}