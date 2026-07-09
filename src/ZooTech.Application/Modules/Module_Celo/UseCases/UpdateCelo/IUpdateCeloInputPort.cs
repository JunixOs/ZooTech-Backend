namespace ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

public interface IUpdateCeloInputPort
{
    Task<UpdateCeloOutput> Handle(UpdateCeloCommand command, CancellationToken cancellationToken = default);
}
