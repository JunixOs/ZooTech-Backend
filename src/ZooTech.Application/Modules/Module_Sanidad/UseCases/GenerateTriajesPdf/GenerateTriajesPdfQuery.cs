namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

public sealed record GenerateTriajesPdfQuery(
    string? Fecha,
    string? Codigo,
    string? Nombre,
    string? TipoPeso,
    decimal? PesoKg);
