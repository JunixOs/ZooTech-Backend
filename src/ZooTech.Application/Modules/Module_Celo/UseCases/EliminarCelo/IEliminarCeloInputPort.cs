namespace ZooTech.Application.Modules.Module_Celo.UseCases.EliminarCelo;

public interface IEliminarCeloInputPort
{
    Task HandleAsync(EliminarCeloCommand command, CancellationToken cancellationToken = default);
}
