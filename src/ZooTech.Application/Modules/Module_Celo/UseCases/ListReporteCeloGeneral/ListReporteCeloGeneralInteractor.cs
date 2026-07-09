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
        ListReporteCeloGeneralCommand cmd,
        CancellationToken cancellationToken = default)
    {
        var currentPage = cmd.Page <= 0 ? 1 : cmd.Page;
        var currentPageSize = cmd.PageSize <= 0
            ? CeloPaginationDefaults.DefaultPageSize
            : Math.Min(cmd.PageSize, CeloPaginationDefaults.MaxPageSize);

        var (celos, totalCount) = await _celoRepository.GetPagedForReporteAsync(
            cmd.Search,
            currentPage,
            currentPageSize,
            cmd.FechaInicio,
            cmd.FechaFin,
            cmd.ColumnFilters,
            cancellationToken);

        var counts = await _celoRepository.GetVecesEnCeloCountsAsync(cancellationToken);
        var criasCounts = await _celoRepository.GetCriasCountsAsync(cancellationToken);

        var items = celos.Select(c => CeloReporteItemMapper.Map(c, counts, criasCounts)).ToList();

        var result = new PagedResult<CeloReporteItemDto>(items, totalCount, currentPage, currentPageSize);
        return new ListReporteCeloGeneralOutput(result);
    }
}
