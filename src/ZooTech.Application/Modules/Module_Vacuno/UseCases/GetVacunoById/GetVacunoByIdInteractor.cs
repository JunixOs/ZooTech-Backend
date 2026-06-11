using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;

public sealed class GetVacunoByIdInteractor : IGetVacunoByIdInputPort
{
    private readonly IVacunoRepository _repository;

    public GetVacunoByIdInteractor(IVacunoRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetVacunoByIdOutput> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var vacuno = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("No se encontró el vacuno solicitado.");

        return new GetVacunoByIdOutput(VacunoAppMapper.ToOutput(vacuno));
    }
}
