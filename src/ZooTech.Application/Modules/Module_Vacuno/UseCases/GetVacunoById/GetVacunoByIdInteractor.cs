using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;

public sealed class GetVacunoByIdInteractor : IGetVacunoByIdInputPort
{
    private readonly IVacunoRepository _repository;

    public GetVacunoByIdInteractor(IVacunoRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetVacunoByIdOutput> HandleAsync(GetVacunoByIdQuery query, CancellationToken cancellationToken)
    {
        var vacuno = await _repository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new VacunoNotFoundException($"No existe un vacuno con el ID {query.Id}.");
        return new GetVacunoByIdOutput(VacunoAppMapper.ToOutput(vacuno));
    }
}
