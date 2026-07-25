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

        var items = celos.Select(c => new CeloItemDto
        {
            Id = c.Id,

            CodigoRegistro = c.Codigo,
            Fecha = DateOnly.FromDateTime(c.FechaHora),
            Hora = TimeOnly.FromDateTime(c.FechaHora),
            CodigoVacuno = c.VacunoCodigo,
            NombreVacuno = c.NombreVacuno,
            VecesEnCelo = counts.GetValueOrDefault(c.VacunoId, 1),
            Observaciones = c.Observaciones,
            CaracteristicaCodes = c.CaracteristicaCodes,
        }).ToList();

        return new GetCelosOutput(items);
    }
}
