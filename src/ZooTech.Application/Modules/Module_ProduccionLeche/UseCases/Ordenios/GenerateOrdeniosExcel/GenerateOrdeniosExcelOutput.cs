namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public sealed record GenerateOrdeniosExcelOutput(
    byte[] Content,
    string ContentType,
    string FileName);

