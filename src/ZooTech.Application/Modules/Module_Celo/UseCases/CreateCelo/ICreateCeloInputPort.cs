namespace ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

public interface ICreateCeloInputPort
{
    Task<CreateCeloOutput> Handle(CreateCeloCommand command, CancellationToken cancellationToken);
}
