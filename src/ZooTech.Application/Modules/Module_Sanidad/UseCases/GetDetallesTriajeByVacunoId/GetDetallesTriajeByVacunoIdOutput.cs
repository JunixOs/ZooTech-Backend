using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId
{
public sealed record GetDetallesTriajeByVacunoIdOutput(
    IReadOnlyList<TriajeDetallePorVacunoItem> Items);
}