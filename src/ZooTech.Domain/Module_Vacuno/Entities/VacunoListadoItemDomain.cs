namespace ZooTech.Domain.Module_Vacuno.Entities;

public sealed record VacunoListadoItemDomain(
    long Id,
    string Codigo,
    DateOnly FechaRegistro,
    string Nombre,
    DateOnly FechaNacimiento,
    string RazaCode,
    string SexoCode,
    string? Raza,
    string? Procedencia,
    string? Estado);

public sealed record ListarVacunosCriteriaDomain(
    DateOnly FechaDesde,
    DateOnly FechaHasta,
    string? Q,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    int Page,
    int Limit);
