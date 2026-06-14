
using ZooTech.Application.Common.Configuration;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosInteractor : IListarVacunosInputPort
{
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IVacunosConfiguration _settings;

    public ListarVacunosInteractor(IVacunoRepository vacunoRepository, IVacunosConfiguration settings)
    {
        _vacunoRepository = vacunoRepository;
        _settings = settings;
    }

    public async Task<ListarVacunosOutput> HandleAsync(ListarVacunosCommand command, CancellationToken cancellationToken = default)
    {
        var fechaDesde = command.FechaDesde;
        if (!fechaDesde.HasValue && !command.FechaHasta.HasValue)
        {
            fechaDesde = DateTime.UtcNow.AddDays(-_settings.DefaultFilterDays);
        }

        var (items, totalCount) = await _vacunoRepository.GetPagedAsync(
            command.Query,
            fechaDesde,
            command.FechaHasta,
            command.Page,
            command.Limit,
            cancellationToken);

        var dtos = items.Select(x => new VacunoItemDto(
            Id: x.Vacuno.Id,
            Codigo: x.Vacuno.Codigo,
            Nombre: x.Vacuno.Nombre,
            FechaNacimiento: x.Vacuno.FechaNacimiento,
            RazaCode: x.Vacuno.RazaCode,
            SexoCode: x.Vacuno.SexoCode,
            Procedencia: x.Procedencia,
            IsDeleted: x.Vacuno.IsDeleted)).ToList();

        return new ListarVacunosOutput(dtos, totalCount);
    }
}
