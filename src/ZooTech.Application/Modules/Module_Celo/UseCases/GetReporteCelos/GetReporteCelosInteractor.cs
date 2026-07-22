using ZooTech.Application.Common.Models;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

public sealed class GetReporteCelosInteractor : IGetReporteCelosInputPort
{
    private readonly ICeloRepository _celoRepository;

    public GetReporteCelosInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<GetReporteCelosOutput> HandleAsync(
        EmptyCommand emptyCommand,
        CancellationToken cancellationToken = default
    )
    {
        var celos = await _celoRepository.GetAllForReporteAsync(cancellationToken);
        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);
        var criasCounts = await _celoRepository.GetCriasCountsAsync(cancellationToken);

        var items = celos.Select(c => CeloReporteItemMapper.Map(c, counts, criasCounts)).ToList();

        return new GetReporteCelosOutput(items);
    }
}
