namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

public sealed record GenerateTriajesPdfOutput(
    byte[] Content,
    string ContentType,
    string FileName);
