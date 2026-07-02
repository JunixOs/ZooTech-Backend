namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public sealed record GetAllTriajesQuery(
    int Pagina,
    int Tamano,
    string? FechaInicio = null,
    string? FechaFin = null,
    string? Codigo = null,
    string? Nombre = null,
    string? TipoPeso = null,
    decimal? PesoKg = null);
