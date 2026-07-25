using ZooTech.Application.Common.Models;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

public sealed class GetCelosInteractor : IGetCelosInputPort
{
    private readonly ICeloRepository _celoRepository;

    public GetCelosInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<GetCelosOutput> Handle(
        EmptyCommand emptyCommand,
        CancellationToken cancellationToken = default
    )
    {
        var celos = await _celoRepository.GetAllAsync(cancellationToken);
        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);

        var items = celos.Select(c => CeloItemDtoMapper.Map(c, counts)).ToList();

        return new GetCelosOutput(items);
    }
}
