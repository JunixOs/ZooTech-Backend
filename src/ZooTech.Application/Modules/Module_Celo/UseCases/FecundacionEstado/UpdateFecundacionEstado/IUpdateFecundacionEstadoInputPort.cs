namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public interface IUpdateFecundacionEstadoInputPort
{
    Task<UpdateFecundacionEstadoOutput> HandleAsync(
        UpdateFecundacionEstadoCommand command,
        CancellationToken cancellationToken = default);
}
