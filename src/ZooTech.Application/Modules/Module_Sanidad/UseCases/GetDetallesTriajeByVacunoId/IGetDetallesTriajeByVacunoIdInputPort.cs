namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId
{
    public interface IGetDetallesTriajeByVacunoIdInputPort
    {
        Task<GetDetallesTriajeByVacunoIdOutput> HandleAsync(GetDetallesTriajeByVacunoIdCommand cmd, CancellationToken cancellationToken = default);
    }
}