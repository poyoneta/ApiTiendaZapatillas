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

        public async Task<Variante> CrearVarianteAsync(Variante variante)
        {
            _context.Variantes.Add(variante);
            await _context.SaveChangesAsync();
            return variante;
        }

        public async Task<Imagen> SubirImagenAsync(IFormFile archivo, int orden, int? idZapatilla, int? idVariante)
        {
            int? zapatillaId = (idZapatilla == 0) ? null : idZapatilla;
            int? varianteId = (idVariante == 0) ? null : idVariante;

            // Validamos que la zapatilla/variante exista ANTES de tocar el bucket —
            // así nunca subimos un archivo que después no podamos asociar a nada.
            if (zapatillaId.HasValue)
            {
                bool existeZapatilla = await _context.Zapatillas.AnyAsync(z => z.Id == zapatillaId.Value);
                if (!existeZapatilla)
                    throw new InvalidOperationException($"No existe una zapatilla con Id={zapatillaId.Value}.");
            }

            if (varianteId.HasValue)
            {
                bool existeVariante = await _context.Variantes.AnyAsync(v => v.Id == varianteId.Value);
                if (!existeVariante)
                    throw new InvalidOperationException($"No existe una variante con Id={varianteId.Value}.");
            }

            string urlPublica = await _storageService.SubirArchivoAsync(archivo);

            try
            {
                var nuevaImagen = new Imagen
                {
                    Url = urlPublica,
                    Orden = orden,
                    Id_zapatilla = zapatillaId,
                    Id_variante = varianteId
                };

                _context.Imagenes.Add(nuevaImagen);
                await _context.SaveChangesAsync();

                return nuevaImagen;
            }
            catch
            {
                // Red de seguridad: si por cualquier otro motivo falla el guardado
                // en base, no dejamos el archivo huérfano en el bucket.
                await _storageService.BorrarArchivoAsync(urlPublica);
                throw;
            }
        }

        public async Task<bool> EliminarZapatillaAsync(int id)
        {
            var zapatilla = await _context.Zapatillas
                .Include(z => z.Imagenes)
                .Include(z => z.Variantes)
                    .ThenInclude(v => v.Imagenes)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zapatilla == null)
                return false;

            // Juntamos las imágenes propias de la zapatilla + las de todas sus variantes
            var todasLasImagenes = zapatilla.Imagenes
                .Concat(zapatilla.Variantes.SelectMany(v => v.Imagenes))
                .ToList();

            // Borramos primero los archivos del bucket de Supabase Storage
            foreach (var imagen in todasLasImagenes)
            {
                await _storageService.BorrarArchivoAsync(imagen.Url);
            }

            // Borramos la zapatilla — el cascade en el DbContext se encarga
            // de las filas de Imagen y Variante en la base
            _context.Zapatillas.Remove(zapatilla);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarVarianteAsync(int id)
        {
            var variante = await _context.Variantes
                .Include(v => v.Imagenes)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (variante == null)
                return false;

            // Borramos primero los archivos del bucket que pertenecen a ESTA variante puntual
            foreach (var imagen in variante.Imagenes)
            {
                await _storageService.BorrarArchivoAsync(imagen.Url);
            }

            _context.Variantes.Remove(variante);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}