namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public record ListarVacunosCommand(
    string? Query = null,
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    string? Estado = null,
    int Page = 1,
    int Limit = 20);
