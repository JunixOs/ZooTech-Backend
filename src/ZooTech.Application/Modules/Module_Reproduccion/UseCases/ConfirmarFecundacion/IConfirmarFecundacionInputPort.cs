namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.ConfirmarFecundacion;

public interface IConfirmarFecundacionInputPort
{
    Task<ConfirmarFecundacionOutput> HandleAsync(ConfirmarFecundacionCommand command, CancellationToken cancellationToken = default);
}
