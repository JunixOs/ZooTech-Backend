using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosInteractor : IListarVacunosInputPort
{
    private readonly IVacunoRepository _vacunoRepository;

    public ListarVacunosInteractor(IVacunoRepository vacunoRepository)
    {
        _vacunoRepository = vacunoRepository;
    }

    public async Task<ListarVacunosOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var vacunos = await _vacunoRepository.ListAllForDisplayAsync(cancellationToken);

        var items = vacunos.Select(x => new VacunoItemDto(
            Id: x.Vacuno.Id,
            Codigo: x.Vacuno.Codigo,
            Nombre: x.Vacuno.Nombre,
            FechaNacimiento: x.Vacuno.FechaNacimiento,
            RazaCode: x.Vacuno.RazaCode,
            SexoCode: x.Vacuno.SexoCode,
            Procedencia: x.Procedencia,
            IsDeleted: x.Vacuno.IsDeleted)).ToList();

        return new ListarVacunosOutput(items);
    }
}
