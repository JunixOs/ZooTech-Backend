namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed record ReportAnimalListOutput(
    DateOnly FechaInicio,
    DateOnly FechaFin,
    string? Keyword,
    IReadOnlyCollection<ReportAnimalListItem> Items);
