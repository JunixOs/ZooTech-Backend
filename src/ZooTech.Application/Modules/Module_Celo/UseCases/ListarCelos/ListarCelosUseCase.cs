using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;

public sealed class ListarCelosUseCase : IListarCelosUseCase
{
    private readonly ICeloRepository _celoRepository;

    public ListarCelosUseCase(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<List<CeloListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var celos = await _celoRepository.ListarCelosAsync(cancellationToken);

        var vecesEnCeloPorVacuno = celos
            .GroupBy(c => c.VacunoId)
            .ToDictionary(g => g.Key, g => g.Count());

        return celos.Select(c => new CeloListItemDto
        {
            CodigoRegistro = c.Codigo,
            Fecha = DateOnly.FromDateTime(c.FechaHora),
            Hora = TimeOnly.FromDateTime(c.FechaHora),
            CodigoVacuno = c.VacunoCodigo,
            NombreVacuno = c.NombreVacuno,
            VecesEnCelo = vecesEnCeloPorVacuno.GetValueOrDefault(c.VacunoId, 1)
        }).ToList();
    }
}
