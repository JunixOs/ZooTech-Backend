using ZooTech.Application.Common.Pagination;
using ZooTech.Application.Modules.Module_Celo.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;

public sealed class ListReporteCeloGeneralInteractor : IListReporteCeloGeneralInputPort
{
    private readonly ICeloRepository _celoRepository;

    public ListReporteCeloGeneralInteractor(ICeloRepository celoRepository)
    {
        _celoRepository = celoRepository;
    }

    public async Task<ListReporteCeloGeneralOutput> HandleAsync(
        ListReporteCeloGeneralQuery query,
        CancellationToken cancellationToken = default)
    {
        var currentPage = query.Page <= 0 ? 1 : query.Page;
        var currentPageSize = query.PageSize <= 0
            ? CeloPaginationDefaults.DefaultPageSize
            : Math.Min(query.PageSize, CeloPaginationDefaults.MaxPageSize);

        var (celos, totalCount) = await _celoRepository.GetPagedForReporteAsync(
            query.Search,
            currentPage,
            currentPageSize,
            query.FechaInicio,
            query.FechaFin,
            query.ColumnFilters,
            cancellationToken);

        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);
        var criasCounts = await _celoRepository.GetCriasCountsAsync(cancellationToken);

        var items = celos.Select(c => CeloReporteItemMapper.Map(c, counts, criasCounts)).ToList();

        var result = new PagedResult<CeloReporteItemDto>(items, totalCount, currentPage, currentPageSize);
        return new ListReporteCeloGeneralOutput(result);
    }
}
