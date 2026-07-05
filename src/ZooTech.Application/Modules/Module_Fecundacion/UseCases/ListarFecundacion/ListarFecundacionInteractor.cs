using System;
using System.Collections.Generic;
using System.Text;
using ZooTech.Application.Common.Configuration;
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public sealed class ListarFecundacionInteractor : IListarFecundacionInputPort
{
    private readonly IFecundacionRepository _fecundacionRepository;
    private readonly IVacunosConfiguration _settings;

    public ListarFecundacionInteractor(IFecundacionRepository fecundacionRepository, IVacunosConfiguration settings)
    {
        _fecundacionRepository = fecundacionRepository;
        _settings = settings;
    }

    public async Task<ListarFecundacionOutput> HandleAsync(
        ListarFecundacionCommand command, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _fecundacionRepository.GetPagedAsync(
            command.Query,
            command.FechaDesde,
            command.FechaHasta,
            command.Resultado,
            command.Page,
            command.Limit,
            cancellationToken);

        return new ListarFecundacionOutput(items, totalCount);
    }
}
