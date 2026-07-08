using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoCatalogs;
using ZooTech.Domain.Module_Vacuno.Models;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.GetVacunoCatalogs
{
    public class GetVacunoCatalogsBehaviorPipelineFactory : IGetVacunoCatalogsBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommand , VacunoCatalogs> _logging;
        private readonly AuditBehavior<EmptyCommand , VacunoCatalogs> _audit;

        private readonly IGetVacunoCatalogsInputPort _handler;

        public GetVacunoCatalogsBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommand , VacunoCatalogs> logging,
            AuditBehavior<EmptyCommand , VacunoCatalogs> audit,

            IGetVacunoCatalogsInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommand , VacunoCatalogs> Create()
        {
            return new BehaviorPipeline<EmptyCommand , VacunoCatalogs>(
            [
                _logging,
                _audit
            ],
            _handler.HandleAsync
            );
        }
    }
}