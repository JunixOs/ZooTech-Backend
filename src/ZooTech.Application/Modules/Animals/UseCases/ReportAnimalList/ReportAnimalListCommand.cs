namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed record ReportAnimalListCommand(
    DateOnly? FechaInicio,
    DateOnly? FechaFin,
    string? Keyword,
    bool ExportExcel);
