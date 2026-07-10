using ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

public sealed record GenerateTriajesPdfDocument(
    IReadOnlyList<TriajeOutput> Items,
    string? Fecha,
    string? FechaDesde,
    string? FechaHasta,
    string? Codigo,
    string? Nombre,
    string? TipoPeso,
    decimal? PesoKg,
    long? VacunoId,
    DateTime GeneratedAtUtc);
