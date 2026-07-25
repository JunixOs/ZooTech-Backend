using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Celo.GetHistorialCeloPorVacuno;

public sealed class GetHistorialCeloPorVacunoBehaviorPipelineFactory(
    LoggingBehavior<GetHistorialCeloPorVacunoCommand, GetHistorialCeloPorVacunoOutput> logging,
    AuditBehavior<GetHistorialCeloPorVacunoCommand, GetHistorialCeloPorVacunoOutput> audit,
    IGetHistorialCeloPorVacunoInputPort handler) : IGetHistorialCeloPorVacunoBehaviorPipelineFactory
{
    public BehaviorPipeline<GetHistorialCeloPorVacunoCommand, GetHistorialCeloPorVacunoOutput> Create() =>
        new([logging, audit], handler.HandleAsync);
}
