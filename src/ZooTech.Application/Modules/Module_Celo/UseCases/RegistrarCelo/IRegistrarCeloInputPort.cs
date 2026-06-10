namespace ZooTech.Application.Modules.Module_Celo.UseCases.RegistrarCelo;

public interface IRegistrarCeloInputPort
{
    Task<RegistrarCeloOutput> HandleAsync(RegistrarCeloCommand command, CancellationToken cancellationToken = default);
}
