using System.Collections.Generic;
using ZooTech.Application.Modules.Module_Fecundacion.Common;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;

public sealed record ListarFecundacionesPaginadoOutput(
    List<FecundacionItemDto> Data,
    int Page,
    int Limit,
    int Total,
    int TotalPages);
