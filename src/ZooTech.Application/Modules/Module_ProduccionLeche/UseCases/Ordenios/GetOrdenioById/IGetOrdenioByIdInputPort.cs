namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;

public interface IGetOrdenioByIdInputPort
{
    Task<GetOrdenioByIdOutput> Handle(GetOrdenioByIdCommand cmd, CancellationToken cancellationToken);
}
