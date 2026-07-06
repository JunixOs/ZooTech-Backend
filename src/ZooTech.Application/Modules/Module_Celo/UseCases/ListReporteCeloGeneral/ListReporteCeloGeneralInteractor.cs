using ZooTech.Application.Common.Pagination;
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

        var (celos, totalCount) = await _celoRepository.GetPagedForReporteAsync(
            search,
            currentPage,
            currentPageSize,
            fechaInicio,
            fechaFin,
            columnFilters,
            cancellationToken);

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

        var result = new PagedResult<CeloReporteItemDto>(items, totalCount, currentPage, currentPageSize);
        return new ListReporteCeloGeneralOutput(result);
    }
}
