namespace ZooTech.Application.Common.Gateway.Features;

/// <summary>
/// Contrato para almacenamiento de archivos.
/// Desacopla el caso de uso del mecanismo concreto (disco local, S3, etc.).
/// </summary>
public interface IArchivoService
{
    /// <summary>
    /// Almacena el archivo y retorna la ruta relativa persistida.
    /// </summary>
    /// <param name="stream">Contenido del archivo.</param>
    /// <param name="nombreOriginal">Nombre original del archivo subido.</param>
    /// <param name="carpeta">Subcarpeta destino (ej: "vacunos/fotos").</param>
    Task<string> GuardarAsync(
        Stream stream,
        string nombreOriginal,
        string carpeta,
        CancellationToken ct = default);
}