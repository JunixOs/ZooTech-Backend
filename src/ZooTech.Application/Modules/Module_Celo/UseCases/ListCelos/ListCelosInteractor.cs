using ZooTech.Application.Common.Pagination;
using ZooTech.Application.Modules.Module_Celo.Models;
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
        ListCelosQuery query,
        CancellationToken cancellationToken = default)
    {
        var currentPage = query.Page <= 0 ? 1 : query.Page;
        var currentPageSize = query.PageSize <= 0
            ? CeloPaginationDefaults.DefaultPageSize
            : Math.Min(query.PageSize, CeloPaginationDefaults.MaxPageSize);

        var (celos, totalCount) = await _celoRepository.GetPagedAsync(
            query.Search,
            currentPage,
            currentPageSize,
            query.FechaInicio,
            query.FechaFin,
            query.ColumnFilters,
            cancellationToken);

        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);

        var items = celos.Select(c => CeloItemDtoMapper.Map(c, counts)).ToList();

        var result = new PagedResult<CeloItemDto>(items, totalCount, currentPage, currentPageSize);
        return new ListCelosOutput(result);
    }
}
