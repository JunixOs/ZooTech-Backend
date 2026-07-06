using ZooTech.Domain.Module_Vacuno.ReadModels.ListarVacuno;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed record ListarVacunosOutput(IReadOnlyList<VacunoListItem> Items, int TotalCount);
