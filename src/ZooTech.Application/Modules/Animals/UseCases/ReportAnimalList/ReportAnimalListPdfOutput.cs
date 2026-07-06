namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed record ReportAnimalListPdfOutput(
    string FileName,
    string ContentType,
    byte[] Content);
