namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public sealed record GenerateOrdeniosComparationExcelQuery(
    long? VacunoId,
    string? EstadoOrdenioCode,
    DateTime? FechaDesde,
    DateTime? FechaHasta);
