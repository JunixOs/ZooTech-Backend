namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed record ListarVacunosQuery(
    string? Q,
    string? Estado,
    DateOnly? FechaDesde,
    DateOnly? FechaHasta);
