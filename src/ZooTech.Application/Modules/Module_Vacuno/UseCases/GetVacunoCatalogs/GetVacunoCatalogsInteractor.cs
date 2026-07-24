using ZooTech.Application.Common.Models;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.Models;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoCatalogs;

public sealed class GetVacunoCatalogsInteractor : IGetVacunoCatalogsInputPort
{
    private readonly IVacunoRepository _repository;

    public GetVacunoCatalogsInteractor(IVacunoRepository repository)
    {
        _repository = repository;
    }

    public Task<VacunoCatalogs> HandleAsync(EmptyCommand emptyCommand, CancellationToken cancellationToken = default)
        => _repository.GetCatalogsAsync(cancellationToken);
}
