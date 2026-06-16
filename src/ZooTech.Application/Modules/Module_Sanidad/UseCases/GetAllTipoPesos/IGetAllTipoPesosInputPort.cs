namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

public interface IGetAllTipoPesosInputPort
{
    Task<GetAllTipoPesosOutput> HandleAsync(CancellationToken cancellationToken = default);
}
