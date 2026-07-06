using ZooTech.Application.Common.Pagination;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;

public sealed class ListCelosInteractor : IListCelosInputPort
{
    private readonly ICeloRepository _celoRepository;

    public ListCelosInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<ListCelosOutput> HandleAsync(
        string? search,
        int page,
        int pageSize,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        IReadOnlyDictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        var currentPage = page <= 0 ? 1 : page;
        var currentPageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var (celos, totalCount) = await _celoRepository.GetPagedAsync(
            search,
            currentPage,
            currentPageSize,
            fechaInicio,
            fechaFin,
            columnFilters,
            cancellationToken);

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
        }).ToList();

        var result = new PagedResult<CeloItemDto>(items, totalCount, currentPage, currentPageSize);
        return new ListCelosOutput(result);
    }
}
