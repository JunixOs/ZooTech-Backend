using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllVacunosSanidad
{
    public class GetAllVacunosSanidadBehaviorPipelineFactory : IGetAllVacunosSanidadBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommandQuery , GetAllVacunosSanidadOutput> _logging;
        private readonly AuditBehavior<EmptyCommandQuery , GetAllVacunosSanidadOutput> _audit;

        private readonly IGetAllVacunosSanidadInputPort _handler;

        public GetAllVacunosSanidadBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommandQuery , GetAllVacunosSanidadOutput> logging,
            AuditBehavior<EmptyCommandQuery , GetAllVacunosSanidadOutput> audit,

            IGetAllVacunosSanidadInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommandQuery , GetAllVacunosSanidadOutput> Create()
        {
            return new BehaviorPipeline<EmptyCommandQuery , GetAllVacunosSanidadOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}