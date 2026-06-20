namespace ZooTech.Domain.Module_Vacuno.Criteria;

public sealed record ListarVacunosCriteriaDomain(
    DateOnly? FechaDesde,
    DateOnly? FechaHasta,
    string? Q,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    int Page,
    int Limit);
