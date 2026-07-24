using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed record GenealogiaVacunoReportModel(
    IReadOnlyCollection<VacunoGenealogiaNode> Nodes,
    Vacuno Root);
