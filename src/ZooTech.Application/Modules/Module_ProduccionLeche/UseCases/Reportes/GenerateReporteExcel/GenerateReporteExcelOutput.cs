namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Reportes.GenerateReporteExcel;

public record GenerateReporteExcelOutput
{
    public byte[] ExcelBytes { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public GenerateReporteExcelOutput(byte[] excelBytes, string fileName)
    {
        ExcelBytes = excelBytes;
        FileName = fileName;
    }
}
