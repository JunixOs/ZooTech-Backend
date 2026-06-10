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
        var vacunos = await _vacunoRepository.ListAllAsync(cancellationToken);

        var items = vacunos.Select(v => new VacunoItemDto(
            Id: v.Id,
            Codigo: v.Codigo,
            Nombre: v.Nombre,
            RazaCode: v.RazaCode,
            SexoCode: v.SexoCode)).ToList();

        return new ListarVacunosOutput(items);
    }
}
