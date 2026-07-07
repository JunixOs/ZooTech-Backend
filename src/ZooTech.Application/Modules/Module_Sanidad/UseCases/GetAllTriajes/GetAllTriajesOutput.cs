using ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public sealed record GetAllTriajesOutput(
    IReadOnlyList<TriajeOutput> Data,
    int TotalCount);

