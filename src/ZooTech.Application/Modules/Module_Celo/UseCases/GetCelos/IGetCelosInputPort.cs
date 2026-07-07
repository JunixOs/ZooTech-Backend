namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

public interface IGetCelosInputPort
{
    Task<GetCelosOutput> HandleAsync(CancellationToken cancellationToken = default);
}
    