using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

public sealed class GetAllVacunosSanidadInteractor : IGetAllVacunosSanidadInputPort
{
    private readonly ITriajeRepository _repository;

    public GetAllVacunosSanidadInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllVacunosSanidadOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var vacunos = await _repository.GetAllVacunosAsync(cancellationToken);
        var items = vacunos.Select(v => new VacunoSanidadItemOutput(v.Id, v.Codigo, v.Nombre)).ToList();
        return new GetAllVacunosSanidadOutput(items);
    }
}
