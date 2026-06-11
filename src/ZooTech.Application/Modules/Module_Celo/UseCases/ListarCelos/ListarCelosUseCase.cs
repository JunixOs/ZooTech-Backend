using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;

public sealed class ListarCelosInteractor : IListarCelosInputPort
{
    private readonly ICeloRepository _celoRepository;

    public ListarCelosInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<ListarCelosOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var celos = await _celoRepository.ListarCelosAsync(cancellationToken);

        var vecesEnCeloPorVacuno = celos
            .GroupBy(c => c.VacunoId)
            .ToDictionary(g => g.Key, g => g.Count());

        var items = celos.Select(c => new CeloListItemDto
        {
            CodigoRegistro = c.Codigo,
            Fecha = DateOnly.FromDateTime(c.FechaHora),
            Hora = TimeOnly.FromDateTime(c.FechaHora),
            CodigoVacuno = c.VacunoCodigo,
            NombreVacuno = c.NombreVacuno,
            VecesEnCelo = vecesEnCeloPorVacuno.GetValueOrDefault(c.VacunoId, 1)
        }).ToList();

        return new ListarCelosOutput(items);
    }
}
