namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed record ReportAnimalListExcelOutput(
    string FileName,
    string ContentType,
    byte[] Content);
