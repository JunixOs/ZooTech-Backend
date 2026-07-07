namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

public sealed record GenerateTriajesExcelOutput(
    byte[] Content,
    string ContentType,
    string FileName);
