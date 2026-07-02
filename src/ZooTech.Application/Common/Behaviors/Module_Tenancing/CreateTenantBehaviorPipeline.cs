using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing
{
    public class CreateTenantPipelineFactory
        : ICreateTenantPipelineFactory
    {
        private readonly ValidationBehavior<CreateTenantCommand , CreateTenantOutput> _validation;
        private readonly LoggingBehavior<CreateTenantCommand, CreateTenantOutput> _logging;
        private readonly AuditBehavior<CreateTenantCommand, CreateTenantOutput> _audit;

        private readonly ICreateTenantInputPort _handler;

        public CreateTenantPipelineFactory(
            ValidationBehavior<CreateTenantCommand , CreateTenantOutput> validation,
            LoggingBehavior<CreateTenantCommand, CreateTenantOutput> logging,
            AuditBehavior<CreateTenantCommand, CreateTenantOutput> audit,

            ICreateTenantInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<CreateTenantCommand, CreateTenantOutput> Create()
        {
            return new BehaviorPipeline<CreateTenantCommand, CreateTenantOutput>(
            [
                _validation,
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}