namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

public interface ICreateFecundacionInputPort
{
    Task<CreateFecundacionOutput> HandleAsync(
        CreateFecundacionCommand command,
        CancellationToken cancellationToken = default);
}
