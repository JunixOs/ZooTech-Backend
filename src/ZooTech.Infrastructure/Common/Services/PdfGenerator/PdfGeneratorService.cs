using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Gateway.Services;

namespace ZooTech.Infrastructure.Common.Services.PdfGenerator;

/// <summary>
/// Servicio reutilizable para generar PDFs con QuestPDF
/// Implementa la interfaz de la capa Application (Gateway)
/// </summary>
public class PdfGeneratorService : IPdfGeneratorService
{
    public PdfGeneratorService()
    {
        // Configurar licencia de QuestPDF (Community License)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generate(string title, Action<dynamic> documentBuilder)
    {
        return Document.Create(documentBuilder)
            .GeneratePdf();
    }

    public async Task<byte[]> GenerateAsync(string title, Action<dynamic> documentBuilder, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => Generate(title, documentBuilder), cancellationToken);
    }
}
