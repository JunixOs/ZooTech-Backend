using System;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Modules.Module_Fecundacion.Common;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;

public sealed class ListarFecundacionesPaginadoInteractor : IListarFecundacionesPaginadoInputPort
{
    private readonly IFecundacionQueryRepository _queryRepository;

    public ListarFecundacionesPaginadoInteractor(IFecundacionQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<ListarFecundacionesPaginadoOutput> HandleAsync(
        ListarFecundacionesPaginadoCommand command,
        CancellationToken cancellationToken)
    {
        var (data, total) = await _queryRepository.GetPagedAsync(
            command.Page,
            command.Limit,
            command.FechaDesde,
            command.FechaHasta,
            command.Q,
            command.TipoFecundacion,
            command.Estado,
            command.Responsable,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)total / command.Limit);

        return new ListarFecundacionesPaginadoOutput(
            data,
            command.Page,
            command.Limit,
            total,
            totalPages == 0 ? 1 : totalPages);
    }
}
