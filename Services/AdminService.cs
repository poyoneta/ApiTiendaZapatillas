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

        // Edición parcial: solo se modifican los campos que llegan (los null se dejan como están)
        public async Task<bool> EditarZapatillaAsync(int id, int? marcaId, string? nombre, string? descripcion)
        {
            var zapatilla = await _context.Zapatillas.FindAsync(id);
            if (zapatilla == null)
                return false;

            if (nombre != null)
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("El nombre de la zapatilla no puede estar vacío.");
                zapatilla.Nombre = nombre.Trim();
            }

            if (marcaId.HasValue)
            {
                bool existeMarca = await _context.Marcas.AnyAsync(m => m.Id == marcaId.Value);
                if (!existeMarca)
                    throw new InvalidOperationException($"No existe la marca con Id={marcaId.Value}.");
                zapatilla.MarcaId = marcaId.Value;
            }

            // Para vaciar la descripción se manda "" (null significa "no tocar")
            if (descripcion != null)
                zapatilla.Descripcion = descripcion;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditarVarianteAsync(int id, int? talla, decimal? precio, int? stock)
        {
            var variante = await _context.Variantes.FindAsync(id);
            if (variante == null)
                return false;

            if ((precio.HasValue && precio.Value < 0) || (stock.HasValue && stock.Value < 0))
                throw new InvalidOperationException("El precio y el stock no pueden ser negativos.");

            if (talla.HasValue) variante.Talla = talla.Value;
            if (precio.HasValue) variante.Precio = precio.Value;
            if (stock.HasValue) variante.Stock = stock.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        // Deja como única principal a la imagen indicada (dentro de su mismo colorway)
        public async Task<bool> MarcarImagenPrincipalAsync(int imagenId)
        {
            var imagen = await _context.Imagenes.FindAsync(imagenId);
            if (imagen == null)
                return false;

            var delColorway = await _context.Imagenes
                .Where(i => i.ZapatillaColorId == imagen.ZapatillaColorId)
                .ToListAsync();

            foreach (var i in delColorway)
                i.Es_Principal = (i.Id == imagenId);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarZapatillaColorAsync(int id)
        {
            var colorway = await _context.Zapatilla_Colores
                .Include(zc => zc.Imagenes)
                .FirstOrDefaultAsync(zc => zc.Id == id);

            if (colorway == null)
                return false;

            // Primero los talles (por si la FK de la base no tiene ON DELETE CASCADE)
            var variantes = await _context.Variantes
                .Where(v => v.ZapatillaColorId == id)
                .ToListAsync();
            _context.Variantes.RemoveRange(variantes);

            foreach (var imagen in colorway.Imagenes)
            {
                await _storageService.BorrarArchivoAsync(imagen.Url);
            }

            _context.Zapatilla_Colores.Remove(colorway);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarImagenAsync(int id)
        {
            var imagen = await _context.Imagenes.FindAsync(id);
            if (imagen == null)
                return false;

            bool eraPrincipal = imagen.Es_Principal;
            int colorwayId = imagen.ZapatillaColorId;

            await _storageService.BorrarArchivoAsync(imagen.Url);
            _context.Imagenes.Remove(imagen);
            await _context.SaveChangesAsync();

            // Si borramos la principal, la primera que quede pasa a ser la principal
            if (eraPrincipal)
            {
                var siguiente = await _context.Imagenes
                    .Where(i => i.ZapatillaColorId == colorwayId)
                    .OrderBy(i => i.Orden)
                    .ThenBy(i => i.Id)
                    .FirstOrDefaultAsync();

                if (siguiente != null)
                {
                    siguiente.Es_Principal = true;
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> EliminarZapatillaAsync(int id)
        {
            var zapatilla = await _context.Zapatillas
                .Include(z => z.ZapatillaColores)
                    .ThenInclude(zc => zc.Imagenes)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zapatilla == null)
                return false;

            // Borramos primero las Variantes (talles) de cada colorway: si la FK
            // Variantes -> Zapatilla_Colores no tiene ON DELETE CASCADE en la base,
            // el delete de la zapatilla falla porque todavía quedan talles colgando.
            var zapatillaColorIds = zapatilla.ZapatillaColores.Select(zc => zc.Id).ToList();
            var variantes = await _context.Variantes
                .Where(v => zapatillaColorIds.Contains(v.ZapatillaColorId))
                .ToListAsync();
            _context.Variantes.RemoveRange(variantes);

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