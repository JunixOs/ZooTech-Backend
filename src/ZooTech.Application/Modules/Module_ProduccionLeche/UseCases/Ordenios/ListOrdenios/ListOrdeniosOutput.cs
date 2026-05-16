using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

public sealed record ListOrdeniosOutput(IReadOnlyList<OrdenioOutput> Data);
