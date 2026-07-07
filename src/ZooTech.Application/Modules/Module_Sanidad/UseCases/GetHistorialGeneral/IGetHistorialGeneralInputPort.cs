namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

public interface IGetHistorialGeneralInputPort
{
    Task<GetHistorialGeneralOutput> HandleAsync(string? fechaDesde = null, string? fechaHasta = null, CancellationToken cancellationToken = default);
}
