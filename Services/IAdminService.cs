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

        Task<bool> EditarZapatillaAsync(int id, int? marcaId, string? nombre, string? descripcion);
        Task<bool> EditarVarianteAsync(int id, int? talla, decimal? precio, int? stock);
        Task<bool> MarcarImagenPrincipalAsync(int imagenId);

        Task<bool> EliminarZapatillaColorAsync(int id);
        Task<bool> EliminarImagenAsync(int id);
        Task<bool> EliminarZapatillaAsync(int id);
        Task<bool> EliminarVarianteAsync(int id);
    }
}