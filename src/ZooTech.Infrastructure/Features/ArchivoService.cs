using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Features;

namespace ZooTech.Infrastructure.Features;

/// <summary>
/// Servicio de almacenamiento de archivos en el sistema de ficheros del VPS.
/// La ruta raíz se configura en appsettings.json: "Storage:BasePath".
/// </summary>
public sealed class ArchivoService : IArchivoService
{
    private readonly string _basePath;

    public ArchivoService(IConfiguration configuration)
    {
        _basePath = configuration["Storage:BasePath"]
            ?? throw new InvalidOperationException(
                "Falta la configuración 'Storage:BasePath' en appsettings.json.");
    }

    /// <inheritdoc/>
    public async Task<string> GuardarAsync(
        Stream stream,
        string nombreOriginal,
        string carpeta,
        CancellationToken ct = default)
    {
        // Generar nombre único para evitar colisiones
        var extension = Path.GetExtension(nombreOriginal).ToLowerInvariant();
        var nombreArchivo = $"{Guid.NewGuid():N}{extension}";

        // Construir ruta física completa
        var directorio = Path.Combine(_basePath, carpeta);
        Directory.CreateDirectory(directorio); // crea si no existe

        var rutaFisica = Path.Combine(directorio, nombreArchivo);

        // Escribir en disco
        await using var fileStream = new FileStream(rutaFisica, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream, ct);

        // Retornar ruta relativa (se usa como clave persistida en BD)
        return Path.Combine(carpeta, nombreArchivo).Replace("\\", "/");
    }
}