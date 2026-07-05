namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public sealed record GetAllTriajesQuery(
    int Pagina,
    int Tamano,
    string? Fecha = null,
    string? Codigo = null,
    string? Nombre = null,
    string? TipoPeso = null,
    decimal? PesoKg = null);
