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
        var fechaDesde = command.FechaDesde;
        if (!fechaDesde.HasValue && !command.FechaHasta.HasValue)
        {
            fechaDesde = DateTime.UtcNow.AddDays(-_settings.DefaultFilterDays);
        }

        var (items, totalCount) = await _fecundacionRepository.GetPagedAsync(
            command.Query,
            fechaDesde,
            command.FechaHasta,
            command.Resultado,
            command.Page,
            command.Limit,
            cancellationToken);

        return new ListarFecundacionOutput(items, totalCount);
    }
}