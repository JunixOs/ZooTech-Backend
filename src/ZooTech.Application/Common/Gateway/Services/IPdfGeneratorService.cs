namespace ZooTech.Application.Common.Gateway.Services;

/// <summary>
/// Interfaz para generar documentos PDF de forma genérica
/// Abstracción que permite que la capa Application no dependa de Infrastructure
/// </summary>
public interface IPdfGeneratorService
{
    /// <summary>
    /// Genera un PDF a partir de una acción de QuestPDF
    /// </summary>
    /// <param name="title">Título del documento PDF</param>
    /// <param name="documentBuilder">Acción que construye el documento</param>
    /// <returns>Array de bytes del PDF generado</returns>
    byte[] Generate(string title, Action<dynamic> documentBuilder);

    /// <summary>
    /// Genera un PDF de forma asincrónica
    /// </summary>
    /// <param name="title">Título del documento PDF</param>
    /// <param name="documentBuilder">Acción que construye el documento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Array de bytes del PDF generado</returns>
    Task<byte[]> GenerateAsync(string title, Action<dynamic> documentBuilder, CancellationToken cancellationToken = default);
}
