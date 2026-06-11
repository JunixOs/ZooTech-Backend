namespace ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

public interface ICreateCeloInputPort
{
    Task<CreateCeloOutput> HandleAsync(CreateCeloCommand command, CancellationToken cancellationToken = default);
}
