namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

public sealed record GenerateTriajesExcelQuery(
    string? Fecha,
    string? FechaDesde,
    string? FechaHasta,
    string? Codigo,
    string? Nombre,
    string? TipoPeso,
    decimal? PesoKg,
    long? VacunoId);
