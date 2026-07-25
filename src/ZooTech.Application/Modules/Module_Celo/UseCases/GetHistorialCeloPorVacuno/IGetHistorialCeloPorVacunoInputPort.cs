namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;

public interface IGetHistorialCeloPorVacunoInputPort
{
    Task<GetHistorialCeloPorVacunoOutput> HandleAsync(GetHistorialCeloPorVacunoCommand command, CancellationToken cancellationToken = default);
}
