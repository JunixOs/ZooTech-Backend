using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoCatalogs;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Models;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.GetVacunoCatalogs
{
    public class GetVacunoCatalogsBehaviorPipelineFactory : IGetVacunoCatalogsBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommandQuery , VacunoCatalogs> _logging;
        private readonly AuditBehavior<EmptyCommandQuery , VacunoCatalogs> _audit;

        private readonly IGetVacunoCatalogsInputPort _handler;

        public GetVacunoCatalogsBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommandQuery , VacunoCatalogs> logging,
            AuditBehavior<EmptyCommandQuery , VacunoCatalogs> audit,

            IGetVacunoCatalogsInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommandQuery , VacunoCatalogs> Create()
        {
            return new BehaviorPipeline<EmptyCommandQuery , VacunoCatalogs>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}