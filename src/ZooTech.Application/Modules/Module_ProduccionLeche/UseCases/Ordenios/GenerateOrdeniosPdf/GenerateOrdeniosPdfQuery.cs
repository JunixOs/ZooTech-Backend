namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public sealed record GenerateOrdeniosPdfQuery(
    long? VacunoId,
    string? EstadoOrdenioCode,
    DateTime? FechaDesde,
    DateTime? FechaHasta);
