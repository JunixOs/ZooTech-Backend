using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

public sealed class GetReporteCelosInteractor : IGetReporteCelosInputPort
{
    private readonly ICeloRepository _celoRepository;

    public GetReporteCelosInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<GetReporteCelosOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var celos = await _celoRepository.GetAllForReporteAsync(cancellationToken);
        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);
        var criasCounts = await _celoRepository.GetCriasCountsAsync(cancellationToken);

        var items = celos.Select(c => new CeloReporteItemDto
        {
            CodigoRegistro = c.Codigo,
            Fecha = DateOnly.FromDateTime(c.FechaHora),
            Hora = TimeOnly.FromDateTime(c.FechaHora),
            CodigoVacuno = c.VacunoCodigo,
            NombreVacuno = c.NombreVacuno,
            VecesEnCelo = counts.GetValueOrDefault(c.VacunoId, 1),
            Caracteristicas = c.CaracteristicaCodes.Count,
            ListaCaracteristicas = c.CaracteristicaCodes,
            Observaciones = c.Observaciones,
            Crias = criasCounts.GetValueOrDefault(c.VacunoId, 0)
        }).ToList();

        return new GetReporteCelosOutput(items);
    }
}
