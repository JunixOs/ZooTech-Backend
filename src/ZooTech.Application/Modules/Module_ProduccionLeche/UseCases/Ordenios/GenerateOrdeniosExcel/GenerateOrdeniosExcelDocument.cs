using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public sealed record GenerateOrdeniosExcelDocument(
    IReadOnlyList<OrdenioListOutput> Items,
    IReadOnlyList<ExcelColumnDefinition> Columns,
    string? SheetName,
    long? VacunoId,
    string? EstadoOrdenioCode,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    DateTime GeneratedAtUtc);

public sealed record ExcelColumnDefinition(string Header, string Key);
