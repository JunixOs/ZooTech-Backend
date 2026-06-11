namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

public interface ICreateOrdenioInputPort
{
    Task<CreateOrdenioOutput> HandleAsync(CreateOrdenioCommand command, CancellationToken cancellationToken);
}
