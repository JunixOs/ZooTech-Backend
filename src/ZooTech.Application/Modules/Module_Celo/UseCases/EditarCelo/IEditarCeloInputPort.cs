namespace ZooTech.Application.Modules.Module_Celo.UseCases.EditarCelo;

public interface IEditarCeloInputPort
{
    Task<EditarCeloOutput> HandleAsync(EditarCeloCommand command, CancellationToken cancellationToken = default);
}
