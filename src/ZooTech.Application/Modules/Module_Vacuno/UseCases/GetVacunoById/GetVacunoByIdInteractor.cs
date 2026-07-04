using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Common;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;

public sealed class GetVacunoByIdInteractor : IGetVacunoByIdInputPort
{
    private readonly IVacunoQueryRepository _queryRepository;

    public GetVacunoByIdInteractor(IVacunoQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<GetVacunoByIdOutput> HandleAsync(long id, CancellationToken cancellationToken)
    {
        var vacuno = await _queryRepository.GetDetalleByIdAsync(id, cancellationToken)
            ?? throw new VacunoNotFoundException($"No existe un vacuno con el ID {id}.");

        return new GetVacunoByIdOutput(vacuno);
    }
}
