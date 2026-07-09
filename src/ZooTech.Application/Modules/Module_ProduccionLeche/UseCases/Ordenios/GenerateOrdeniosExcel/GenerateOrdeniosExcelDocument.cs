using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public sealed record GenerateOrdeniosExcelDocument(
    IReadOnlyList<OrdenioOutput> Items,
    long? VacunoId,
    string? EstadoOrdenioCode,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    DateTime GeneratedAtUtc);