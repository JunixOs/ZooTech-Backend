namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public sealed record GenerateOrdeniosPdfOutput(
    byte[] Content,
    string ContentType,
    string FileName);
