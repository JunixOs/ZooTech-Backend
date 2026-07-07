namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public interface IUpdateFecundacionInputPort
{
    Task<UpdateFecundacionOutput> HandleAsync(
        UpdateFecundacionCommand command,
        CancellationToken cancellationToken = default);
}
