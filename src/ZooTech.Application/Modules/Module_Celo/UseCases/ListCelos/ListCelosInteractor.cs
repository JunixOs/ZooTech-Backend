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
        ListCelosCommand cmd,
        CancellationToken cancellationToken = default)
    {
        var currentPage = cmd.Page <= 0 ? 1 : cmd.Page;
        var currentPageSize = cmd.PageSize <= 0
            ? CeloPaginationDefaults.DefaultPageSize
            : Math.Min(cmd.PageSize, CeloPaginationDefaults.MaxPageSize);

        var (celos, totalCount) = await _celoRepository.GetPagedAsync(
            cmd.Search,
            currentPage,
            currentPageSize,
            cmd.FechaInicio,
            cmd.FechaFin,
            cmd.ColumnFilters,
            cancellationToken);

        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);

        var items = celos.Select(c => CeloItemDtoMapper.Map(c, counts)).ToList();

        var result = new PagedResult<CeloItemDto>(items, totalCount, currentPage, currentPageSize);
        return new ListCelosOutput(result);
    }
}
