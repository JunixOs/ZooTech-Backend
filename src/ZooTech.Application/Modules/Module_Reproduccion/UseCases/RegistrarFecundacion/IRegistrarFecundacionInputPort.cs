namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;

public interface IRegistrarFecundacionInputPort
{
    Task<RegistrarFecundacionOutput> HandleAsync(RegistrarFecundacionCommand command, CancellationToken cancellationToken = default);
}
