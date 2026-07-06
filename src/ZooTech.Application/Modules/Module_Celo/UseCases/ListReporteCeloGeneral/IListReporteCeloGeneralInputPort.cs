namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;

public interface IListReporteCeloGeneralInputPort
{
    Task<ListReporteCeloGeneralOutput> HandleAsync(
        string? search,
        int page,
        int pageSize,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        CancellationToken cancellationToken = default);
}
