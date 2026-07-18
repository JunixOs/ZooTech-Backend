namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public interface IUpdateOrdenioInputPort
{
    Task<UpdateOrdenioOutput> Handle(UpdateOrdenioCommand command, CancellationToken cancellationToken);
}
