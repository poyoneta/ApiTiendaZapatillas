using ApiTiendaZapas.Data;
using ApiTiendaZapas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApiTiendaZapas.Services
{
    public class AdminService : IAdminService
    {
        private readonly ZapatillasContext _context;
        private readonly IStorageService _storageService;

        public AdminService(ZapatillasContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<Marca> CrearMarcaAsync(Marca marca)
        {
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();
            return marca;
        }

        public async Task<Color> CrearColorAsync(Color color)
        {
            _context.Colores.Add(color);
            await _context.SaveChangesAsync();
            return color;
        }

        public async Task<Zapatilla> CrearZapatillaAsync(Zapatilla zapatilla)
        {
            _context.Zapatillas.Add(zapatilla);
            await _context.SaveChangesAsync();
            return zapatilla;
        }

        public async Task<ZapatillaColor> CrearZapatillaColorAsync(ZapatillaColor zapatillaColor)
        {
            _context.Zapatilla_Colores.Add(zapatillaColor);
            await _context.SaveChangesAsync();
            return zapatillaColor;
        }

        public async Task<Variante> CrearVarianteAsync(Variante variante)
        {
            _context.Variantes.Add(variante);
            await _context.SaveChangesAsync();
            return variante;
        }

        public async Task<Imagen> SubirImagenAsync(IFormFile archivo, int orden, bool esPrincipal, int zapatillaColorId)
        {
            bool existeZapatillaColor = await _context.Zapatilla_Colores.AnyAsync(zc => zc.Id == zapatillaColorId);
            if (!existeZapatillaColor)
                throw new InvalidOperationException($"No existe la relación ZapatillaColor con Id={zapatillaColorId}.");

            string urlPublica = await _storageService.SubirArchivoAsync(archivo);

            try
            {
                var nuevaImagen = new Imagen
                {
                    Url = urlPublica,
                    Orden = orden,
                    Es_Principal = esPrincipal,
                    ZapatillaColorId = zapatillaColorId
                };

                _context.Imagenes.Add(nuevaImagen);
                await _context.SaveChangesAsync();

                return nuevaImagen;
            }
            catch
            {
                await _storageService.BorrarArchivoAsync(urlPublica);
                throw;
            }
        }

        public async Task<bool> EliminarZapatillaAsync(int id)
        {
            var zapatilla = await _context.Zapatillas
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Imagenes)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zapatilla == null)
                return false;

            // Obtenemos las imágenes a través de los colorways
            var todasLasImagenes = zapatilla.ZapatillaColores
                .SelectMany(zc => zc.Imagenes)
                .ToList();

            foreach (var imagen in todasLasImagenes)
            {
                await _storageService.BorrarArchivoAsync(imagen.Url);
            }

            _context.Zapatillas.Remove(zapatilla);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarVarianteAsync(int id)
        {
            var variante = await _context.Variantes.FindAsync(id);

            if (variante == null)
                return false;

            _context.Variantes.Remove(variante);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}