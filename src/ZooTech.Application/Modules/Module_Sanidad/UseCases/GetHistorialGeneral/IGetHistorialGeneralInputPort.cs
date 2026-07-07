namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

public interface IGetHistorialGeneralInputPort
{
    Task<GetHistorialGeneralOutput> HandleAsync(GetHistorialGeneralCommand cmd, CancellationToken cancellationToken = default);
}
