using ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

public sealed record GenerateTriajesExcelDocument(
    IReadOnlyList<TriajeOutput> Items,
    string? Fecha,
    string? Codigo,
    string? Nombre,
    string? TipoPeso,
    decimal? PesoKg,
    DateTime GeneratedAtUtc);