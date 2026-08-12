using ApiTiendaZapas.Data;
using ApiTiendaZapas.Repositories;
using ApiTiendaZapas.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args
});

// FIX: Evita el crash "inotify instance limit reached" en contenedores Linux (Render)
// desactivando el watcher de cambios en appsettings.json (no lo necesitamos en producción)
builder.Configuration.Sources
    .OfType<Microsoft.Extensions.Configuration.Json.JsonConfigurationSource>()
    .ToList()
    .ForEach(s => s.ReloadOnChange = false);

// 1. CONFIGURACIÓN DE SERVICIOS
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiTiendaZapas", Version = "v1" });
    c.RequestBodyFilter<XFormFileFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ZapatillasContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        }
    )
);

// Repositories y Services (arquitectura en capas)
builder.Services.AddScoped<IZapatillaRepository, ZapatillaRepository>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IAdminService, AdminService>();

var app = builder.Build();

// 2. PIPELINE DE LA APLICACIÓN (MIDDLEWARES)
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Frontend");
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();

public class XFormFileFilter : IRequestBodyFilter
{
    public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
    {
    }
}