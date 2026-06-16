namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed record ReportAnimalListCommand(
    DateOnly? FechaInicio,
    DateOnly? FechaFin,
    string? Keyword,
    string? RazaCode,
    string? SexoCode,
    string? TipoAdquisicionCode,
    long? GranjaId,
    string? EstadoCode,
    bool ExportExcel,
    bool ExportPdf);
