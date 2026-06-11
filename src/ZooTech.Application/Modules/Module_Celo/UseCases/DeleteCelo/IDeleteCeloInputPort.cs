namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

public interface IDeleteCeloInputPort
{
    Task HandleAsync(DeleteCeloCommand command, CancellationToken cancellationToken = default);
}
