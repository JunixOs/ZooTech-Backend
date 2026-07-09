using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

namespace ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllVacunosSanidad
{
    public class GetAllVacunosSanidadBehaviorPipelineFactory : IGetAllVacunosSanidadBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommand , GetAllVacunosSanidadOutput> _logging;
        private readonly AuditBehavior<EmptyCommand , GetAllVacunosSanidadOutput> _audit;

        private readonly IGetAllVacunosSanidadInputPort _handler;

        public GetAllVacunosSanidadBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommand , GetAllVacunosSanidadOutput> logging,
            AuditBehavior<EmptyCommand , GetAllVacunosSanidadOutput> audit,

            IGetAllVacunosSanidadInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommand , GetAllVacunosSanidadOutput> Create()
        {
            return new BehaviorPipeline<EmptyCommand , GetAllVacunosSanidadOutput>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}