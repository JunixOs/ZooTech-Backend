namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;

public interface IListReporteCeloGeneralInputPort
{
    Task<ListReporteCeloGeneralOutput> HandleAsync(
        ListReporteCeloGeneralCommand cmd,
        CancellationToken cancellationToken = default);
}
