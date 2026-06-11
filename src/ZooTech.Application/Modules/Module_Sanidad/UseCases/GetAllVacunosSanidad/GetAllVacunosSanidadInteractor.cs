using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

public sealed class GetAllVacunosSanidadInteractor : IGetAllVacunosSanidadInputPort
{
    private readonly IVacunoRepository _vacunoRepository;

    public GetAllVacunosSanidadInteractor(IVacunoRepository vacunoRepository)
    {
        _vacunoRepository = vacunoRepository;
    }

    public async Task<GetAllVacunosSanidadOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var vacunos = await _vacunoRepository.ListAllAsync(cancellationToken);
        var items = vacunos.Select(v => new VacunoSanidadItemOutput(v.Id, v.Codigo, v.Nombre)).ToList();
        return new GetAllVacunosSanidadOutput(items);
    }
}
