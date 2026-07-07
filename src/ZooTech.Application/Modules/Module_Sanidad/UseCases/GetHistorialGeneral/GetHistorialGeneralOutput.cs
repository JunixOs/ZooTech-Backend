using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

public sealed record GetHistorialGeneralOutput(IReadOnlyList<HistorialTriajeItemOutput> Items);
