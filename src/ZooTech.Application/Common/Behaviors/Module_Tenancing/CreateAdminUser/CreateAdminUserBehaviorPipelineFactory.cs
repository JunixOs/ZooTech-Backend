using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser.Ports;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateAdminUser
{
    public class CreateAdminUserBehaviorPipelineFactory : ICreateAdminUserBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<CreateAdminUserCommand, CreateAdminUserOutput> _validation;
        private readonly LoggingBehavior<CreateAdminUserCommand, CreateAdminUserOutput> _logging;
        private readonly AuditBehavior<CreateAdminUserCommand, CreateAdminUserOutput> _audit;

        private readonly ICreateAdminUserInputPort _handler;

        public CreateAdminUserBehaviorPipelineFactory(
            ValidationBehavior<CreateAdminUserCommand, CreateAdminUserOutput> validation,
            LoggingBehavior<CreateAdminUserCommand, CreateAdminUserOutput> logging,
            AuditBehavior<CreateAdminUserCommand, CreateAdminUserOutput> audit,

            ICreateAdminUserInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<CreateAdminUserCommand, CreateAdminUserOutput> Create()
        {
            return new BehaviorPipeline<CreateAdminUserCommand, CreateAdminUserOutput>(
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