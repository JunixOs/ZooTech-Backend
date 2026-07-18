namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;

public interface IGetFecundacionEstadoInputPort
{
    Task<GetFecundacionEstadoOutput> HandleAsync(
        GetFecundacionEstadoCommand command,
        CancellationToken cancellationToken = default);
}
