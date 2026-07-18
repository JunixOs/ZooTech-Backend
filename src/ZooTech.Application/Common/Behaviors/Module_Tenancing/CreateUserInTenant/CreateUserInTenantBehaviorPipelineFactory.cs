using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant.Ports;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateUserInTenant
{
    public class CreateUserInTenantBehaviorPipelineFactory : ICreateUserInTenantBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<CreateUserInTenantCommand, CreateUserInTenantOutput> _validation;
        private readonly LoggingBehavior<CreateUserInTenantCommand, CreateUserInTenantOutput> _logging;
        private readonly AuditBehavior<CreateUserInTenantCommand, CreateUserInTenantOutput> _audit;

        private readonly ICreateUserInTenantInputPort _handler;

        public CreateUserInTenantBehaviorPipelineFactory(
            ValidationBehavior<CreateUserInTenantCommand, CreateUserInTenantOutput> validation,
            LoggingBehavior<CreateUserInTenantCommand, CreateUserInTenantOutput> logging,
            AuditBehavior<CreateUserInTenantCommand, CreateUserInTenantOutput> audit,

            ICreateUserInTenantInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<CreateUserInTenantCommand, CreateUserInTenantOutput> Create()
        {
            return new BehaviorPipeline<CreateUserInTenantCommand, CreateUserInTenantOutput>(
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