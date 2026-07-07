namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed record ReportAnimalListOutput(
    DateOnly FechaInicio,
    DateOnly FechaFin,
    string? Keyword,
    string? RazaCode,
    string? ColorCode,
    string? SexoCode,
    string? TipoAdquisicionCode,
    long? GranjaId,
    string? EstadoCode,
    IReadOnlyCollection<ReportAnimalListItem> Items);
