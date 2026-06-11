namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed record ListarVacunosCommand(
    string? FechaDesde,
    string? FechaHasta,
    string? Q,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    string? Formato,
    string? Page,
    string? Limit);
