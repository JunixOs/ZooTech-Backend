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
        return new ListarVacunosOutput(vacunos);
    }
}
