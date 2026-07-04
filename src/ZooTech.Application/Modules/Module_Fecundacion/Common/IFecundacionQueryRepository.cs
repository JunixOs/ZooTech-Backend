using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZooTech.Application.Modules.Module_Fecundacion.Common;

public interface IFecundacionQueryRepository
{
    Task<(List<FecundacionItemDto> Data, int Total)> GetPagedAsync(
        int page,
        int limit,
        DateOnly? fechaDesde,
        DateOnly? fechaHasta,
        string? q,
        string? tipoFecundacion,
        string? estado,
        string? responsable,
        CancellationToken cancellationToken = default);
}
