using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

public sealed class GetCelosInteractor : IGetCelosInputPort
{
    private readonly ICeloRepository _celoRepository;

    public GetCelosInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<GetCelosOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var celos = await _celoRepository.GetAllAsync(cancellationToken);
        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);

        var items = celos.Select(c => new CeloItemDto
        {
            CodigoRegistro = c.Codigo,
            Fecha = DateOnly.FromDateTime(c.FechaHora),
            Hora = TimeOnly.FromDateTime(c.FechaHora),
            CodigoVacuno = c.VacunoCodigo,
            NombreVacuno = c.NombreVacuno,
            VecesEnCelo = counts.GetValueOrDefault(c.VacunoId, 1),
        }).ToList();

        return new GetCelosOutput(items);
    }
}
