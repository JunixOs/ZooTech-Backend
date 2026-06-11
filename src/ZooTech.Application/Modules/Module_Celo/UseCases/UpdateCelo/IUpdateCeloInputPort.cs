namespace ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

public interface IUpdateCeloInputPort
{
    Task<UpdateCeloOutput> HandleAsync(UpdateCeloCommand command, CancellationToken cancellationToken = default);
}
