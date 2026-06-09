using ZooTech.Domain.Entities.Configuration;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReportePdf;

/// <summary>
/// Output con los bytes del PDF generado
/// </summary>
public record GenerateReportePdfOutput
{
    public byte[] PdfBytes { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = "application/pdf";

    public GenerateReportePdfOutput(byte[] pdfBytes, string fileName)
    {
        PdfBytes = pdfBytes;
        FileName = fileName;
        ContentType = "application/pdf";
    }
}
